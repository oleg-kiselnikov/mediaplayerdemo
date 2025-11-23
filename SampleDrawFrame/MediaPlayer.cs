using MFORMATSLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SampleDrawFrame
{
    public enum MediaPlayerState { None, Playing, Paused, FastForward, Rewind, Disposed }

    public sealed class MediaPlayer : IMediaPlayer, IDisposable
    {
        public bool IsOpened => State == MediaPlayerState.Playing || State == MediaPlayerState.Paused;
        public bool IsPlaying => State == MediaPlayerState.Playing;

        private MediaPlayerState state;
        private MediaPlayerState State
        {
            get { return state; }
            set
            {
                state = value;
                OnStateChanged();
                ContinueHandling();
            }
        }
        private M_AV_PROPS mavProps = new M_AV_PROPS();

        private CancellationTokenSource cts;
        private Task task;

        private readonly MFReaderClass mfReader = new MFReaderClass();
        private readonly MFPreviewClass mfPreview = new MFPreviewClass();

        private IntPtr? handle = null;
        public IntPtr? Handle
        {
            get { return handle; }
            set
            {
                handle = value;

                if (handle.HasValue)
                    mfPreview.PreviewWindowSet("", handle.Value.ToInt32());
            }
        }

        public MediaPlayer()
        {
            State = MediaPlayerState.None;
            mavProps.vidProps.eVideoFormat = eMVideoFormat.eMVF_Custom;


            mfPreview.PreviewEnable("", 0, 1);
            mfPreview.PropsSet("maintain_ar", "false");
        }

        public void Play(string filePath)
        {
            if (State == MediaPlayerState.Disposed)
                throw new ObjectDisposedException(nameof(MediaPlayer));

            if (cts != null)
            {
                if (!cts.IsCancellationRequested)
                    cts.Cancel();
                if (task != null)
                    task.Wait();
            }

            State = MediaPlayerState.Playing;

            cts = new CancellationTokenSource();

            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            task = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await OpenAsync(filePath);

                    await HandleAsync();
                }
                catch { }
                finally
                {
                    State = MediaPlayerState.None;

                    cts.Dispose();
                    cts = null;
                    task = null;
                }
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        public void Stop()
        {
            cts?.Cancel();
        }

        public bool CanSwitchPlayback()
        {
            switch (State)
            {
                case MediaPlayerState.Playing:
                case MediaPlayerState.Paused:
                    return true;

                default:
                    return false;
            }
        }

        public bool SwitchPlayback()
        {
            switch (State)
            {
                case MediaPlayerState.Playing:
                    State = MediaPlayerState.Paused;
                    return true;

                case MediaPlayerState.Paused:
                    State = MediaPlayerState.Playing;
                    return true;

                default:
                    return false;
            }
        }

        public bool CanFastForward()
        {
            switch (State)
            {
                case MediaPlayerState.Playing:
                case MediaPlayerState.Paused:
                    return true;

                default:
                    return false;
            }
        }


        public bool FastForward()
        {
            switch (State)
            {
                case MediaPlayerState.Playing:
                case MediaPlayerState.Paused:
                    State = MediaPlayerState.FastForward;
                    return true;

                default:
                    return false;
            }
        }

        public bool CanRewind()
        {
            switch (State)
            {
                case MediaPlayerState.Playing:
                case MediaPlayerState.Paused:
                    return true;

                default:
                    return false;
            }
        }


        public bool Rewind()
        {
            switch (State)
            {
                case MediaPlayerState.Playing:
                case MediaPlayerState.Paused:
                    State = MediaPlayerState.Rewind;
                    return true;

                default:
                    return false;
            }
        }

        public void ForseFrameRefreshOnPause()
        {
            if (State == MediaPlayerState.Paused)
                ContinueHandling();
        }

        MFFrame frame = null;
        Bitmap bitmap = null;

        private async Task HandleAsync()
        {
            if (cts.Token.IsCancellationRequested)
            {
                ReleaseFrame(ref frame);
                ReleaseBitmap(ref bitmap);
                return;
            }

            switch (State)
            {
                case MediaPlayerState.Playing:
                case MediaPlayerState.FastForward:
                case MediaPlayerState.Rewind:
                case MediaPlayerState.Paused:
                    ReleaseFrame(ref frame);
                    ReleaseBitmap(ref bitmap);
                    break;
            }


            switch (State)
            {
                case MediaPlayerState.Playing:
                    frame = await FastForwardAsync();
                    break;
                case MediaPlayerState.FastForward:
                    frame = await FastForwardAsync();
                    break;

                case MediaPlayerState.Rewind:
                    frame = await RewindAsync();
                    break;

                case MediaPlayerState.Paused:
                    frame = await PauseAsync();
                    break;
            }

            if (frame != null)
            {
                switch (State)
                {
                    case MediaPlayerState.Playing:
                    case MediaPlayerState.FastForward:
                    case MediaPlayerState.Rewind:
                    case MediaPlayerState.Paused:

                        Clone(ref frame);
                        bitmap = ToBitmap(ref frame);
                        OnBeforeRendering(bitmap);
                        await RenderAsync(frame);
                        break;
                }
            }

            switch (State)
            {
                case MediaPlayerState.Playing:
                    break;
                case MediaPlayerState.FastForward:
                case MediaPlayerState.Rewind:
                    State = MediaPlayerState.Paused;
                    break;
                default:
                    await WaitAsync(cts.Token, 300); /*Интервал задан пока в качестве костыля для рисования в режиме Паузы*/
                    break;
            }

            await HandleAsync();
        }

        private bool ReleaseFrame(ref MFFrame frame)
        {
            if (frame == null)
                return false;

            Marshal.ReleaseComObject(frame);
            frame = null;

            return true;
        }

        private bool ReleaseBitmap(ref Bitmap bitmap)
        {
            if (bitmap == null)
                return false;

            bitmap.Dispose();
            bitmap = null;

            return true;
        }

        private TaskCompletionSource<object> tcs = null;
        private Task WaitAsync(CancellationToken cancellationToken, int delay = 0)
        {
            tcs = new TaskCompletionSource<object>(cancellationToken);

            if (delay > 0)
            {
                var awaiter = Task.Delay(delay).GetAwaiter();
                awaiter.OnCompleted(() => tcs?.TrySetResult(null));
            }

            return tcs.Task;
        }

        private void ContinueHandling()
        {
            tcs?.TrySetResult(null);
        }

        private Task OpenAsync(string filePath)
        {
            return Task.Run(() => mfReader.ReaderOpen(filePath, "loop=true"));
        }

        private Task<MFFrame> FastForwardAsync()
        {
            return Task.Run(() =>
            {
                mfReader.SourceFrameConvertedGetByTime(ref mavProps, -1, -1, out MFFrame frame, "loop=true");
                return frame;
            });
        }

        private Task<MFFrame> RewindAsync()
        {
            return Task.Run(() =>
            {
                mfReader.SourceFrameConvertedGetByTime(ref mavProps, -1, -1, out MFFrame frame, " reverse=true");
                return frame;
            });
        }

        private Task<MFFrame> PauseAsync()
        {
            return Task.Run(() =>
            {
                mfReader.SourceFrameConvertedGetByTime(ref mavProps, -1, -1, out MFFrame frame, "pause=true");
                return frame;
            });
        }

        private Task RenderAsync(MFFrame frame)
        {
            return Task.Run(() => mfPreview.ReceiverFramePut(frame, -1, ""));
        }

        private void Clone(ref MFFrame frame)
        {
            frame.MFClone(out MFFrame clonedFrame, eMFrameClone.eMFC_Reference, eMFCC.eMFCC_ARGB32);

            Marshal.ReleaseComObject(frame);

            frame = clonedFrame;
        }

        private Bitmap ToBitmap(ref MFFrame frame)
        {
            frame.MFVideoGetBytes(out int cbSize, out long pbVideo);
            frame.MFAVPropsGet(out M_AV_PROPS avProps, out int audioSample);

            var v = avProps.vidProps;
            var f = PixelFormat.Format32bppRgb;

            return new Bitmap(v.nWidth, Math.Abs(v.nHeight), v.nRowBytes, f, new IntPtr(pbVideo));
        }

        public void Dispose()
        {
            if (State == MediaPlayerState.Disposed)
                return;

            Stop();

            mfReader.ReaderClose();

            State = MediaPlayerState.Disposed;
        }

        private void OnBeforeRendering(Bitmap bitmap)
        {
            BeforeRendering?.Invoke(this, new BeforeRenderingEventArgs() { Bitmap = bitmap });
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler<BeforeRenderingEventArgs> BeforeRendering;
        public event EventHandler StateChanged;
    }
}
