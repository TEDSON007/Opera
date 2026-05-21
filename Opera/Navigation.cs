//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows;


using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Opera;
using System.IO;
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
        if (sender is Microsoft.Web.WebView2.Wpf.WebView2 webView)
        {
            Common.DebugWriteLine("webView");
            var currentTab = NPTabCon.NPTabs.FirstOrDefault(t => t.Content == webView);
            currentTab?.Header = webView.CoreWebView2?.DocumentTitle ?? "Default Title";
            Common.DebugWriteLine(currentTab?.Header.ToString() ?? "No title");

            var newUri = webView.CoreWebView2?.Source;
            if(Excluded(newUri)) { return; }
            if (newUri is not null && !AlreadyExists(newUri))
            {
                _visiedPages.Add(new HistoryItem( newUri));
            }
        }
    }

    private bool Excluded(string? newUri)
    {
        if(newUri is null || newUri.Contains("search")) { return true; }
        return false;
    }

    private bool AlreadyExists(string newUri)
    {
        try
        {
            var res = _visiedPages.First(p => p.Address == newUri);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void WebView_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
    {
        if (sender is Microsoft.Web.WebView2.Wpf.WebView2 webView)
        {
            var json = args.WebMessageAsJson;
            // Example JSON: {"action":"delete","ids":["id1","id2"]}
            var message = System.Text.Json.JsonSerializer.Deserialize<DeleteMessage>(json);

            if (message is not null && message.Action == "delete")
            {
                foreach (var id in message.Ids)
                {
                    var item = _visiedPages.FirstOrDefault(h => h.Id == id);
                    if (item is not null)
                    {
                        _visiedPages.Remove(item);
                    }
                }
                // Optionally regenerate HTML after deletion
                string newHtml = HtmlGenerator.GenerateHistoryHtml(_visiedPages);
                string startPagePath = Path.Combine(Common.AppFolder, "StartPage.html");

                File.WriteAllText(startPagePath, newHtml);
                webView.CoreWebView2.Navigate(startPagePath);
            }
        }
    }

    public class DeleteMessage
{
    public string Action { get; set; }
    public string[] Ids { get; set; }
}

private void WebView_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
    {
        Common.DebugWriteLine("WebView_NavigationStarting");
    }
}
