using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Media;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace Opera;

public partial class MainWindow : Window
{
    #region Fields
    List<BrowserControl> _browserControls = new List<BrowserControl>();
    List<string> _browserHistory = new List<string>();
    #endregion

    #region Properties

    #endregion

    #region Dependancies
    // Does this eed to be a dependecy? TODO
    public ObservableCollection<TabItem> TabItems
    {
        get { return (ObservableCollection<TabItem>)GetValue(TabItemsProperty); }
        set { SetValue(TabItemsProperty, value); }
    }
    public static readonly DependencyProperty TabItemsProperty =
        DependencyProperty.Register(nameof(TabItems), 
            typeof(ObservableCollection<TabItem>), 
            typeof(MainWindow), 
            new PropertyMetadata(new ObservableCollection<TabItem>()));
    #endregion

    #region Constructors
    public MainWindow()
    {
        Loaded += MainWindow_Loaded;
        InitializeComponent();
    }
    #endregion

    #region Methods

    #endregion

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine($"Path: {Common.AppFolder}");
        var browserControl = new BrowserControl("https://stackoverflow.com");
        TabItem tabItem = new()
        {
            Header = browserControl.Browser?.Source?.ToString() ?? "Start",
            Content = browserControl,
        };
        
        _browserControls.Add(browserControl);
        TabItems.Add(tabItem);
        Tabs.ItemsSource = TabItems;
        Tabs.SelectedItem = tabItem;
        browserControl.NavComplete += BrowserControl_NavComplete;
        browserControl.SettingsClicked += BrowserControl_SettingsClicked;
    }

    private void BrowserControl_SettingsClicked(BrowserControl obj)
    {
        SettingsPanel settingsPanel = new SettingsPanel();
        settingsPanel.CloseRequested += SettingsPanel_CloseRequested;
        TabItem settingsTab = new()
        {
            Header = "Settings",
            Content = settingsPanel,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        TabItems.Add(settingsTab);
        Tabs.SelectedItem = settingsTab;
    }

    private void SettingsPanel_CloseRequested(SettingsPanel obj)
    {
        
        var st = TabItems.FirstOrDefault(t => t.Content == obj);
        TabItems.Remove(st);
        obj = null;
        st = null;
    }

    private void BrowserControl_NavComplete(BrowserControl obj)
    {
        var v = TabItems.FirstOrDefault(t => t.Content == obj);
        string newUri = obj.Browser.CoreWebView2.Source;
        v.Header = newUri;
        if (!_browserHistory.Contains(newUri))
        {
            _browserHistory.Add(newUri); 
        }
    }
}