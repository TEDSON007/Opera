using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Opera.Controls;

public partial class NPToggleButton : UserControl
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), 
            typeof(string), 
            typeof(NPToggleButton), 
            new PropertyMetadata("Property"));

    public double ScaleHeight
    {// probably not needed TODO
        get { return (double)GetValue(ScaleHeightProperty); }
        set 
        { 
            SetValue(ScaleHeightProperty, value); 
        }
    }
    public static readonly DependencyProperty ScaleHeightProperty =
        DependencyProperty.Register(nameof(ScaleHeight), typeof(double), typeof(NPToggleButton), new PropertyMetadata(30.0));

    public bool IsSet { get; set; } = false;

    public NPToggleButton()
    {
        ScaleHeight = 30;
        InitializeComponent();
    }

    private void Btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (ToggleSwitch.Fill == Brushes.Blue)
        {
            IsSet = false;
            ToggleSwitch.Fill = Brushes.Gray;
            ToggleSwitch.HorizontalAlignment = HorizontalAlignment.Left;
            return;
        }
        IsSet = true;
        ToggleSwitch.Fill = Brushes.Blue;
        ToggleSwitch.HorizontalAlignment = HorizontalAlignment.Right;
    }
}
