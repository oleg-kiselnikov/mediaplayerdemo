using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SampleDrawFrame
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DrawingArea.Window.Activate();

            var mediaPlayer = new MediaPlayer();

            

            var openFileDialog = new OpenMediaFileDialog();

            var mediaViewModel = new MediaViewModel(mediaPlayer, openFileDialog);

            var drawingViewModel = new DrawingViewModel(mediaPlayer);

            var mainViewModel = new MainViewModel(mediaViewModel, drawingViewModel);


            mediaPlayer.StateChanged += (s, e) =>
            {
                mediaViewModel.IsPlaying = mediaPlayer.IsPlaying;
                mediaViewModel.IsOpened = mediaPlayer.IsOpened;

                CommandManager.InvalidateRequerySuggested();
            };


            DataContext = mainViewModel;
        }
    }
}
