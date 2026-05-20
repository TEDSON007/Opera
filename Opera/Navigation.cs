//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows;


using Microsoft.Web.WebView2.Wpf;
using System.Windows;

namespace Opera;

public partial class MainWindow
{
    private void btnNavBack_Click(object sender, RoutedEventArgs e)
    {
        if(NPTabCon.SelectedItem is NPTab nPTab)
        {
            if (nPTab.WebView.CanGoBack)
            {
                nPTab.WebView.GoBack(); 
            }
        }
        //Browser.CoreWebView2.Navigate(History[_currentHistoryIndex - 1]);
    }

    private void btnNavForward_Click(object sender, RoutedEventArgs e)
    {
        if (NPTabCon.SelectedItem is NPTab nPTab)
        {
            if (nPTab.WebView.CanGoForward)
            {
                nPTab.WebView.GoForward();
            }
        }
        //Browser.CoreWebView2.Navigate(History[_currentHistoryIndex + 1]);
    }

    private void btnNavRefresh_Click(object sender, RoutedEventArgs e)
    {
        if (NPTabCon.SelectedItem is NPTab nPTab)
        {
            nPTab.WebView.Reload();
        }
        //Browser.CoreWebView2.Reload();
    }

    private void btnSearch_Click(object sender, RoutedEventArgs e)
    {
        if (NPTabCon.SelectedItem is NPTab nPTab)
        {
            var uri = $"https://www.google.com/search?q={lbxAddress.Text}";
            if (!string.IsNullOrWhiteSpace(uri))
            {
                nPTab.WebView.CoreWebView2.Navigate(uri); 
            }
        }
        //Browser.CoreWebView2.Navigate(uri);
    }

    private void WebView_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
    {
        Common.DebugWriteLine("WebView_NavigationCompleted");
        if (sender is WebView2 webView)
        {
            Common.DebugWriteLine("webView");
            var currentTab = NPTabCon.NPTabs.FirstOrDefault(t => t.Content == webView);
            currentTab?.Header = webView.CoreWebView2?.DocumentTitle ?? "Default Title";
            Common.DebugWriteLine(currentTab?.Header.ToString() ?? "No title"); // TODO always No Title

            var newUri = webView.CoreWebView2?.Source;
            if (newUri is not null && !_browserHistory.Contains(newUri))
            {
                _browserHistory.Add(newUri);
            }
        }
    }

    private void WebView_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
    {
        Common.DebugWriteLine("WebView_NavigationStarting");
    }
}
