using System;

namespace SampleDrawFrame
{
    public interface IMediaPlayer
    {
        IntPtr? Handle { get; set; }

        bool IsOpened { get; }
        bool IsPlaying { get; }

        void Play(string filePath);
        void Stop();

        bool CanSwitchPlayback();
        bool SwitchPlayback();

        bool CanFastForward();
        bool FastForward();

        bool CanRewind();
        bool Rewind();

        void ForseFrameRefreshOnPause();

        event EventHandler<BeforeRenderingEventArgs> BeforeRendering;
        event EventHandler StateChanged;
    }
}
