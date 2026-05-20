using System.IO;
using System.Windows;
using System.Windows.Controls;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Opera;

public class NPTab : TabItem
{
    #region Fields
    string _startPage = "https://google.com";
    int _currentHistoryIndex = 0;
    int _maxHistoryIndex = 0;

    string _userDataFolder =
    Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "MyBrowser");

    CoreWebView2EnvironmentOptions _coreWebView2EnvironmentOptions = new();
    #endregion
    #region Properties
    private string Title { get; set; }
    public WebView2 WebView { get; set; }
    private CoreWebView2 WebViewCore2 { get; set; }
    #endregion

    #region Events
    public static event Action? NavStart;
    public static event Action<string>? NavEnd;
    #endregion

    #region Constructors
    public NPTab()
    {
        WebView = new WebView2();
        Content = WebView;
    }

    public NPTab(string source)
    {
        WebView = new WebView2();
        WebView?.Source = new Uri(source);
        Content = WebView;
    }
    #endregion

    #region Methods
    private async void PostInit()
    {// https://chromewebstore.google.com/category/extensions
        try
        {
            //lbxAddress.ItemsSource = History;
            _coreWebView2EnvironmentOptions = new CoreWebView2EnvironmentOptions
            {
                AreBrowserExtensionsEnabled = true
            };

            #region ext
            // TODO later date
            //var options = new CoreWebView2EnvironmentOptions("--load-extension=\"C:\\Extensions\\uBlock\"");

            //var env = await CoreWebView2Environment.CreateAsync(
            //    null,
            //    _userDataFolder,
            //    options);

            //await Browser.EnsureCoreWebView2Async(env);
            #endregion

            #region usr
            var env = await CoreWebView2Environment.CreateAsync(
                null,
                _userDataFolder,
                _coreWebView2EnvironmentOptions);

            await WebView.EnsureCoreWebView2Async(env);
            #endregion


            WebView.CoreWebView2.AddWebResourceRequestedFilter("*",
                CoreWebView2WebResourceContext.All);

            WebView.CoreWebView2.WebResourceRequested += WebView_WebResourceRequested;

            WebView.NavigationStarting += WebView_NavigationStarting;
            WebView.CoreWebView2.NavigationCompleted += WebView_NavigationCompleted;
            WebView.CoreWebView2.Navigate(_startPage);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    public void UpdateTitle(string address)
    {
        string host = new Uri(address).Host;
        //string pageTitle = 
    }
    #endregion

    private void WebView_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        //return;
        string url = e.Request.Uri;

        if (url.Contains("doubleclick") ||
            url.Contains("googlesyndication"))
        {
            e.Response = WebView.CoreWebView2.Environment.CreateWebResourceResponse(
                Stream.Null,
                403,
                "Blocked",
                "");
        }
    }

    private void WebView_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        //btnNavBack.IsEnabled = false;
        //btnNavForward.IsEnabled = false;
        NavStart?.Invoke();
    }

    private async void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        #region Code
        string newUri = WebView.CoreWebView2.Source;
        NavEnd?.Invoke(newUri);

        //if (!History.Contains(newUri))
        //{
        //    History.Add(newUri);
        //}
        //_currentHistoryIndex = History.IndexOf(newUri);
        //if (_currentHistoryIndex <= 0)
        //{
        //    _currentHistoryIndex = 0;
        //}
        //else if (_currentHistoryIndex >= History.Count)
        //{
        //    _currentHistoryIndex = History.Count - 1;
        //}
        //_maxHistoryIndex = History.Count - 1;
        //SetNavButtonStates();
        #endregion

        #region Injection
        var Extensions = BrowserExtension.LoadExtensions();
        foreach (BrowserExtension ext in Extensions)
        {
            if (!string.IsNullOrWhiteSpace(ext.InjectJs))
            {
                string jsPath =
                    Path.Combine(ext.FolderPath, ext.InjectJs);

                if (File.Exists(jsPath))
                {
                    string js = File.ReadAllText(jsPath);

                    await WebView.CoreWebView2.ExecuteScriptAsync(js);
                }
            }
        }
        #endregion
    }

}
