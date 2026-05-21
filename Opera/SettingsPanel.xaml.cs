using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Opera;
public partial class SettingsPanel : UserControl
{
    public event Action<Settings>? CloseRequested;
    public event Action<SettingsPanel>? SettingsUpdate;

    public static Settings Settings { get; private set; }

    public SettingsPanel()
    {
        InitializeComponent();
    }

    public static Settings Load()
    {
        // TODO deserialization
        Settings settings = new Settings();
        Common.DebugWriteLine("Load Settings Not implimemted");
        return settings;
    }

    internal static void Save()
    {
        // TODO serialization
        Common.DebugWriteLine("Save Settings Not implimemted");
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        // TODO Should MainWindow handle saving settins? 
        Save();
        CloseRequested.Invoke(Settings);
    }

    private void tbtnTheme_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // TODO Rethink
        //if(sender is NPToggleButton nPToggleButton)
        //{
        //    Common.DebugWriteLine(nPToggleButton.Text);
        //    SettingsUpdate?.Invoke(this);
        //}
    }

    
}

public class Settings
{
    // TODO Model to save/load settings 

}
