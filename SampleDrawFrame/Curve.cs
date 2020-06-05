using System.Collections.ObjectModel;
using System.Windows;

namespace SampleDrawFrame
{
    public class Curve
    {
        public ObservableCollection<Point> Points { get; private set; }

        public System.Drawing.Pen Pen { get; set; }

        public Curve()
        {
            Points = new ObservableCollection<Point>();
        }
    }
}
