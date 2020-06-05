using System.Collections.ObjectModel;
using System.Windows;

namespace SampleDrawFrame
{
    public interface IDrawingViewModel
    {
        ObservableCollection<Curve> Curves { get; }

        Size DrawingAreaSize { get; set; }
    }
}
