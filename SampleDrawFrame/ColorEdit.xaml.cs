using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SampleDrawFrame
{
    public partial class ColorEdit : UserControl
    {
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, Color); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorEdit), 
                new FrameworkPropertyMetadata(FromKnownColor(System.Drawing.KnownColor.LimeGreen), 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e)=>((ColorEdit)s).AppendValueToList()));

        public ICollection<Color> Colors
        {
            get { return (ICollection<Color>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register("Colors", typeof(ICollection<Color>), typeof(ColorEdit), 
                new PropertyMetadata());


        public ColorEdit()
        {
            InitializeComponent();

            Colors = new ObservableCollection<Color>();

            var knownColors = Enum.GetValues(typeof(System.Drawing.KnownColor));
            foreach (System.Drawing.KnownColor knownColor in knownColors)
                Colors.Add(FromKnownColor(knownColor));
        }

        public static System.Windows.Media.Color FromKnownColor(System.Drawing.KnownColor knownColor)
        {
            var color = System.Drawing.Color.FromKnownColor(knownColor);
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private void AppendValueToList()
        {
            if (Colors == null)
                Colors = new ObservableCollection<Color>();

            if (!Colors.Contains(Color))
                Colors.Add(Color);
        }
    }
}
