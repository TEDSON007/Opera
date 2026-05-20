using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Media;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Documents;
namespace Opera;

public partial class MainWindow : Window
{
    #region Fields
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
        NPTab nPTab = NPTabCon.CreateTab();
        nPTab.WebView.NavigationStarting += WebView_NavigationStarting;
        nPTab.WebView.NavigationCompleted += WebView_NavigationCompleted;

        NPTabCon.SelectedItem = nPTab;
        NPTabCon.ItemsSource = TabItems;

        TabItems.Add(nPTab);
    }

    private void SettingsPanel_CloseRequested(SettingsPanel obj)
    {
        SettingsPanel.Save();
    }

    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
        SettingsPanel settingsPanel = new SettingsPanel();
        Settings settings = SettingsPanel.Load();
        NPTab settingsTab = new()
        {
            Header = "Settings",
            Content = settingsPanel,
        };

        NPTabCon.NPTabs.Add(settingsTab);
        NPTabCon.SelectedItem = settingsTab;
    }
}