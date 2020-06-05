using System;
using System.IO;
using System.Windows.Input;

namespace SampleDrawFrame
{
    public sealed class MediaViewModel : ViewModelBase, IMediaViewModel
    {
        public IntPtr? Handle
        {
            get { return mediaPlayer.Handle; }
            set { mediaPlayer.Handle = value; }
        }

        private string source;

        public string Source
        {
            get { return source; }
            set
            {
                source = value;

                if (File.Exists(source))
                    mediaPlayer.Play(source);

                OnPropertyChanged(nameof(Source));
            }
        }

        private bool isOpened;
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                OnPropertyChanged(nameof(IsOpened));
            }
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set 
            { 
                isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        public ICommand FileOpenCommand { get; private set; }
        public ICommand PlaybackSwitchCommand { get; private set; }
        public ICommand RewindCommand { get; private set; }
        public ICommand FastForwardCommand { get; private set; }

        
        private readonly IMediaPlayer mediaPlayer;
        private readonly IOpenFileDialog openFileDialog;
        public MediaViewModel(IMediaPlayer mediaPlayer, IOpenFileDialog openFileDialog)
        {
            this.mediaPlayer = mediaPlayer;
            this.openFileDialog = openFileDialog;            

            FileOpenCommand = new RelayCommand<IOpenFileDialog>(OpenFile, CanOpenFile);
            PlaybackSwitchCommand = new RelayCommand(SwitchPlayback, CanSwitchPlayback);
            RewindCommand = new RelayCommand(Rewind, CanRewind);
            FastForwardCommand = new RelayCommand(FastForward, CanFastForward);

            mediaPlayer.StateChanged += (s, e) =>
            {
                IsPlaying = mediaPlayer.IsPlaying;
                IsOpened = mediaPlayer.IsOpened;

                CommandManager.InvalidateRequerySuggested();
            };
        }

        public bool CanOpenFile(IOpenFileDialog openFileDialog)
        {
            return (openFileDialog ?? this.openFileDialog) != null;
        }

        public void OpenFile(IOpenFileDialog openFileDialog)
        {
            openFileDialog = openFileDialog ?? this.openFileDialog;
            if (openFileDialog?.ShowDialog() == true)
                Source = openFileDialog.FileName;
        }

        public bool CanSwitchPlayback()
        {
            return IsOpened;
        }

        public void SwitchPlayback()
        {
            if (!IsOpened)
                return;

            mediaPlayer.SwitchPlayback();
        }

        

        public bool CanRewind()
        {
            return mediaPlayer.CanRewind();
        }

        public void Rewind()
        {
            mediaPlayer.Rewind();
        }


        public bool CanFastForward()
        {
            return mediaPlayer.CanFastForward();
        }

        public void FastForward()
        {
            mediaPlayer.FastForward();
        }
    }
}
