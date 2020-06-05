namespace SampleDrawFrame
{
    public interface IMainViewModel
    {
        IMediaViewModel MediaViewModel { get; }

        IDrawingViewModel DrawingViewModel { get; }
    }
}
