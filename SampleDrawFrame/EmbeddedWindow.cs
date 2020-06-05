using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace SampleDrawFrame
{
    public class EmbeddedWindow : Control
    {
        public IntPtr? Handle
        {
            get { return (IntPtr)GetValue(HandleProperty); }
            set { SetValue(HandleProperty, value); }
        }

        public static readonly DependencyProperty HandleProperty =
            DependencyProperty.Register("Handle", typeof(IntPtr?), typeof(EmbeddedWindow),
                new PropertyMetadata());

        public Window Window;

        public EmbeddedWindow()
        {
            Window = new Window()
            {
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
            };

            Window.Loaded += (s, e) => Handle = new WindowInteropHelper(Window).Handle;

            Loaded += (ss, ee) =>
            {
                var presentationSource = PresentationSource.FromVisual(this);

                var mainWindow = Window.GetWindow(this);

                Window.Owner = mainWindow;

                mainWindow.LocationChanged +=
                    (s, e) => UpdatePosition(presentationSource, Window);
                LayoutUpdated +=
                    (s, e) => UpdatePosition(presentationSource, Window);

                mainWindow.LocationChanged +=
                    (s, e) => UpdateSize(Window);
                LayoutUpdated +=
                    (s, e) => UpdateSize(Window);

                IsVisibleChanged +=
                    (s, e) => UpdateVisibility(Window);

                UpdatePosition(presentationSource, Window);
                UpdateSize(Window);
                UpdateVisibility(Window);
            };
        }

        private void UpdateVisibility(Window window)
        {
            if (IsVisible)
                window.Show();
            else
                window.Hide();
        }

        private void UpdatePosition(PresentationSource presentationSource, Window window)
        {
            var windowNewPosition = presentationSource.CompositionTarget
                .TransformFromDevice.Transform(PointToScreen(new Point()));

            window.Left = windowNewPosition.X;
            window.Top = windowNewPosition.Y;
        }

        private void UpdateSize(Window window)
        {
            window.Height = ActualHeight;
            window.Width = ActualWidth;
        }
    }
}
