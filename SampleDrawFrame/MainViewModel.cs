namespace SampleDrawFrame
{
    public sealed class MainViewModel : ViewModelBase, IMainViewModel
    {
        public IMediaViewModel MediaViewModel { get; private set; }

        public IDrawingViewModel DrawingViewModel { get; private set; }

        public MainViewModel(IMediaViewModel mediaViewModel, IDrawingViewModel drawingViewModel)
        {
            MediaViewModel = mediaViewModel;
            DrawingViewModel = drawingViewModel;
        }
    }
}
