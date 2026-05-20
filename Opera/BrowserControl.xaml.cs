//using Microsoft.Web.WebView2.Core;
//using System.Diagnostics;
//using System.IO;
//using System.Media;
//using System.Runtime.CompilerServices;
//using System.Windows;
//using System.Windows.Controls;

//namespace Opera
//{
//    public partial class BrowserControl : UserControl
//    {
//        #region Fields
//        string _startPage = "https://google.com";
//        int _currentHistoryIndex = 0;
//        int _maxHistoryIndex = 0;

//        string _userDataFolder =
//        Path.Combine(
//        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
//        "MyBrowser");

//        CoreWebView2EnvironmentOptions _coreWebView2EnvironmentOptions = new CoreWebView2EnvironmentOptions();
//        #endregion

//        #region Events
//        public event Action<BrowserControl>? NavComplete;
//        public event Action<BrowserControl>? SettingsClicked;
//        #endregion

//        #region Dependancies
//        // Only for this cotrol and this session.
//        // Can't recall why I made this a dependency TODO
//        private List<string> History
//        {
//            get { return (List<string>)GetValue(HistoryProperty); }
//            set { SetValue(HistoryProperty, value); }
//        }
//        private static readonly DependencyProperty HistoryProperty =
//            DependencyProperty.Register(nameof(History),
//                typeof(List<string>), typeof(BrowserControl),
//                new PropertyMetadata(new List<string>()));
//        #endregion

//        #region Constructors
//        public BrowserControl()
//        {
//            PreInit();
//            InitializeComponent();
//            PostInit();
//        }

//        public BrowserControl(string openAtPage)
//        {
//            _startPage = openAtPage;
//            PreInit();
//            InitializeComponent();
//            PostInit();
//        }
//        #endregion

//        #region Methods
//        private async void PostInit()
//        {// https://chromewebstore.google.com/category/extensions
//            try
//            {
//                lbxAddress.ItemsSource = History;
//                _coreWebView2EnvironmentOptions = new CoreWebView2EnvironmentOptions
//                {
//                    AreBrowserExtensionsEnabled = true
//                };

//                #region ext
//                // TODO later date
//                //var options = new CoreWebView2EnvironmentOptions("--load-extension=\"C:\\Extensions\\uBlock\"");

//                //var env = await CoreWebView2Environment.CreateAsync(
//                //    null,
//                //    _userDataFolder,
//                //    options);

//                //await Browser.EnsureCoreWebView2Async(env);
//                #endregion

//                #region usr
//                var env = await CoreWebView2Environment.CreateAsync(
//                    null,
//                    _userDataFolder,
//                    _coreWebView2EnvironmentOptions);

//                await Browser.EnsureCoreWebView2Async(env);
//                #endregion


//                Browser.CoreWebView2.AddWebResourceRequestedFilter("*",
//                    CoreWebView2WebResourceContext.All);

//                Browser.CoreWebView2.WebResourceRequested += Browser_WebResourceRequested;

//                Browser.NavigationStarting += Browser_NavigationStarting;
//                Browser.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
//                Browser.CoreWebView2.Navigate(_startPage);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//        }

//        private void Browser_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
//        {
//            //return;
//            string url = e.Request.Uri;

//            if (url.Contains("doubleclick") ||
//                url.Contains("googlesyndication"))
//            {
//                e.Response = Browser.CoreWebView2.Environment.CreateWebResourceResponse(
//                    Stream.Null,
//                    403,
//                    "Blocked",
//                    "");
//            }
//        }

//        private void Browser_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
//        {
//            btnNavBack.IsEnabled = false;
//            btnNavForward.IsEnabled = false;
//        }

//        private void SetNavButtonStates()
//        {
//            btnNavBack.IsEnabled = true;
//            btnNavForward.IsEnabled = true;

//            if (_currentHistoryIndex == 0)
//            {
//                btnNavBack.IsEnabled = false;
//            }
//            else
//            {
//                btnNavBack.IsEnabled = true;
//            }

//            if (_currentHistoryIndex == _maxHistoryIndex)
//            {
//                btnNavForward.IsEnabled = false;
//            }
//            else
//            {
//                btnNavForward.IsEnabled = true;
//            }
//        }

//        private void NotImplimented([CallerMemberName] string caller = "")
//        {
//            Debug.WriteLine($"Member: {caller} - Not Implimented.");
//        }

//        private async void PreInit()
//        {
//            NotImplimented();
//        }

//        public List<string> GetHistory()
//        {// only provide a copy. TODO make more efficient.
//            List<string> history = new();
//            foreach(string uri in History)
//            {
//                history.Add(uri);
//            }
//            return history;
//        }
//        #endregion
//        private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
//        {
//            #region Code
//            string newUri = Browser.CoreWebView2.Source;
//            NavComplete?.Invoke(this);

//            if (!History.Contains(newUri))
//            {
//                History.Add(newUri); 
//            }
//            _currentHistoryIndex = History.IndexOf(newUri);
//            if( _currentHistoryIndex <= 0 )
//            {
//                _currentHistoryIndex = 0;
//                            }
//            else if(_currentHistoryIndex >= History.Count)
//            {
//                _currentHistoryIndex = History.Count -1;
//            }
//            _maxHistoryIndex = History.Count - 1;
//            SetNavButtonStates();
//            #endregion

//            #region Injection
//            var Extensions = BrowserExtension.LoadExtensions();
//            foreach (BrowserExtension ext in Extensions)
//            {
//                if (!string.IsNullOrWhiteSpace(ext.InjectJs))
//                {
//                    string jsPath =
//                        Path.Combine(ext.FolderPath, ext.InjectJs);

//                    if (File.Exists(jsPath))
//                    {
//                        string js = File.ReadAllText(jsPath);

//                        await Browser.CoreWebView2.ExecuteScriptAsync(js);
//                    }
//                }
//            }
//            #endregion
//        }

//        private void UserControl_Loaded(object sender, RoutedEventArgs e)
//        {
//            NotImplimented();
//        }

//        private void btnNavBack_Click(object sender, RoutedEventArgs e)
//        {
//            Browser.CoreWebView2.Navigate(History[_currentHistoryIndex-1]);
//        }

//        private void btnNavForward_Click(object sender, RoutedEventArgs e)
//        {
//            Browser.CoreWebView2.Navigate(History[_currentHistoryIndex + 1]);
//        }

//        private void btnNavRefresh_Click(object sender, RoutedEventArgs e)
//        {
//            Browser.CoreWebView2.Reload();
//        }

//        private void btnSearch_Click(object sender, RoutedEventArgs e)
//        {
//            var uri = $"https://www.google.com/search?q={lbxAddress.Text}";
//            Browser.CoreWebView2.Navigate(uri);
//        }

//        private void btnSettings_Click(object sender, RoutedEventArgs e)
//        {
//            SettingsClicked?.Invoke(this);
//        }
//    }
//}

        