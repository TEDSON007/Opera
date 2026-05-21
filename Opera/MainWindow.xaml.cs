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
    HistoryModel? MyHistory {  get; set; }
    ObservableCollection<HistoryItem> _visiedPages {  get; set; }
    #endregion

    #region Dependancies
    // TODO Does this eed to be a dependecy? 
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

    

    private void SettingsPanel_CloseRequested(SettingsPanel obj)
    {
        SettingsPanel.Save();
    }

    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
        MyHistory.CreateCurrentHistoryPage();
        return;
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

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        MyHistory = new();
        _visiedPages = MyHistory.CurretHisory();

        NPTab nPTab = NPTabCon.CreateTab(MyHistory);
        nPTab.WebView.NavigationStarting += WebView_NavigationStarting;
        nPTab.WebView.NavigationCompleted += WebView_NavigationCompleted;
        nPTab.WebView.WebMessageReceived += WebView_WebMessageReceived;

        NPTabCon.SelectedItem = nPTab;
        NPTabCon.ItemsSource = TabItems;

        TabItems.Add(nPTab);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        MyHistory?.Save();
    }
}