using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SampleDrawFrame
{
    public enum DrawingAreaMode { Default, Drawing }

    public partial class DrawingArea : EmbeddedWindow
    {
        public DrawingAreaMode Mode
        {
            get { return (DrawingAreaMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(DrawingAreaMode), typeof(DrawingArea),
                new PropertyMetadata(DrawingAreaMode.Default, (s, e) => ((DrawingArea)s).Resubscribe()));


        public IDrawingViewModel Drawing
        {
            get { return (IDrawingViewModel)GetValue(DrawingProperty); }
            set { SetValue(DrawingProperty, value); }
        }

        public static readonly DependencyProperty DrawingProperty =
            DependencyProperty.Register("Drawing", typeof(IDrawingViewModel), typeof(DrawingArea),
                new PropertyMetadata(null, (s, e) => ((DrawingArea)s).Resubscribe()));

        private void Resubscribe()
        {
            clickableArea.SizeChanged += ClickableArea_SizeChanged;
            clickableArea.MouseDown -= PreviewPanel_MouseDown;
            clickableArea.MouseMove -= PreviewPanel_MouseMove;
            clickableArea.MouseUp -= PreviewPanel_MouseUp;

            if (Mode == DrawingAreaMode.Drawing)
            {
                if (Drawing is DrawingViewModel)
                {
                    clickableArea.MouseDown += PreviewPanel_MouseDown;
                    clickableArea.MouseMove += PreviewPanel_MouseMove;
                    clickableArea.MouseUp += PreviewPanel_MouseUp;
                }
            }
        }

        private void ClickableArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Drawing.DrawingAreaSize = e.NewSize;
        }

        private bool isDrawing = false;
        private void PreviewPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Drawing.Curves.Add(new Curve());
            isDrawing = true;
        }

        private void PreviewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing)
                return;

            Drawing.Curves.Last().Points.Add(e.GetPosition(clickableArea));
        }

        private void PreviewPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }

        private readonly Grid clickableArea;

        public DrawingArea()
        {
            clickableArea = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = Brushes.White,
                Opacity=0.05, /*Чтобы окно было кликабельным*/
            };
            Window.AllowsTransparency = true;
            Window.Background = Brushes.Transparent;
            Window.Content = clickableArea;
        }
    }
}
