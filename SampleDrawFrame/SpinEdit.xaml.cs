using System.Windows;
using System.Windows.Controls;

namespace SampleDrawFrame
{
    public partial class SpinEdit : UserControl
    {
        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(SpinEdit), new PropertyMetadata(1));


        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(SpinEdit), new PropertyMetadata(10));


        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(SpinEdit), 
                new PropertyMetadata(5, (s, e) => ((SpinEdit)s).CorrectValue()));

        public SpinEdit()
        {
            InitializeComponent();

            Button_Up.Click += (s, e) => Value++;
            Button_Down.Click += (s, e) => Value--;
        }

        private void CorrectValue()
        {
            if (Value < Min)
                Value = Min;
            else if (Value > Max)
                Value = Max;
        }
    }
}
