using Opera.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Opera;
public partial class SettingsPanel : UserControl
{
    public event Action<SettingsPanel>? CloseRequested;
    public event Action<SettingsPanel>? SettingsUpdate;
    public SettingsPanel()
    {
        InitializeComponent();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(this);
        // MainWindow to handle saving settins TODO
    }

    private void tbtnTheme_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if(sender is NPToggleButton nPToggleButton)
        {
            Common.DebugWriteLine(nPToggleButton.Text);
            SettingsUpdate?.Invoke(this);
        }
    }
}

class Settings
{
    // Model to save/load settings TODO 
}
