using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Opera;

public class HistoryModel
{
    // TODO Untested
    
    static readonly string savePath = Path.Combine(Common.AppFolder, "history.json");

    public ObservableCollection<HistoryItem>? SavedPages { get; set; }

    public HistoryModel()
    {
        Load();
    }

    public void Remove(string address)
    {
        SavedPages?.Remove(SavedPages.FirstOrDefault(hi => hi.Address == address));
    }

    public void Add(string address)
    {
        HistoryItem hi = new()
        {
            Address = address,
            LastVisited = DateTime.Now
        };
        SavedPages?.Add(hi);
    }

    public void CreateCurrentHistoryPage()
    {
        string startPageHTML = HtmlGenerator.GenerateHistoryHtml(SavedPages);
        string startPagePath = Path.Combine(Common.AppFolder, "StartPage.html");
        File.WriteAllText(startPagePath, startPageHTML);
    }

    public ObservableCollection<HistoryItem> CurretHisory()
    {
        return SavedPages ?? [];
    }

    public void Save()
    {
        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        string js = System.Text.Json.JsonSerializer.Serialize(SavedPages, options);
        CreateCurrentHistoryPage();
        File.WriteAllText(savePath, js);
    }

    private void Load()
    {
        if(!File.Exists(savePath))
        {
            SavedPages = new ObservableCollection<HistoryItem>();
            return;
        }
        string js = File.ReadAllText(savePath);
        SavedPages = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<HistoryItem>>(js);
    }
}

public class HistoryItem
{
    [Required]
    public string Address { get; set; }
    public string Id { get; set; } // Unique ID for WebView2 interop
    public DateTime LastVisited { get; set; }
    public string? FaviconUrl { get; set; } // Optional favicon URL

    public HistoryItem(string address)
    {
        Address = address;
        LastVisited = DateTime.Now;
    }

    public HistoryItem()
    {
        
    }

    public void UpdateVisited(DateTime lastVisited)
    {
        LastVisited = lastVisited;
    }

    public override string ToString()
    {
        return Address;
    }
}



public static class HtmlGenerator
{
    public static string GenerateHistoryHtml(ObservableCollection<HistoryItem> historyItems)
    {
        if (historyItems == null) throw new ArgumentNullException(nameof(historyItems));

        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>History</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; background-color: #f9f9f9; }");
        sb.AppendLine("        h2 { margin-top: 30px; color: #333; border-bottom: 1px solid #ccc; padding-bottom: 5px; }");
        sb.AppendLine("        .history-item { display: flex; align-items: center; padding: 10px; margin-bottom: 10px; background-color: #fff; border-radius: 5px; box-shadow: 0 0 5px rgba(0,0,0,0.1); }");
        sb.AppendLine("        .favicon { width: 16px; height: 16px; margin-right: 10px; }");
        sb.AppendLine("        .history-content { display: flex; flex-direction: column; flex: 1; }");
        sb.AppendLine("        .history-item a { font-size: 16px; font-weight: bold; color: #007acc; text-decoration: none; }");
        sb.AppendLine("        .history-item a:hover { text-decoration: underline; }");
        sb.AppendLine("        .last-visited { font-size: 12px; color: #555; margin-top: 2px; }");
        sb.AppendLine("        .delete-btn { margin-left: 10px; padding: 2px 6px; font-size: 12px; cursor: pointer; }");
        sb.AppendLine("        .controls { margin-bottom: 15px; }");
        sb.AppendLine("        .controls label { margin-right: 10px; cursor: pointer; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <h1>Browsing History</h1>");
        sb.AppendLine("    <div class=\"controls\">");
        sb.AppendLine("        <label><input type=\"checkbox\" id=\"checkAll\" onclick=\"toggleAll(this)\"> Check All</label>");
        sb.AppendLine("        <button onclick=\"deleteChecked()\">Delete Checked</button>");
        sb.AppendLine("    </div>");

        var sortedItems = historyItems.OrderByDescending(h => h.LastVisited);
        var grouped = sortedItems.GroupBy(h => h.LastVisited.Date);

        foreach (var group in grouped)
        {
            sb.AppendLine($"    <h2>{group.Key:dddd, MMMM d, yyyy}</h2>");
            foreach (var item in group)
            {
                string id = item.Id ?? Guid.NewGuid().ToString();
                item.Id = id; // ensure the ID is set

                sb.AppendLine($"    <div class=\"history-item\" data-id=\"{id}\">");

                sb.AppendLine($"        <input type=\"checkbox\" class=\"item-checkbox\">");

                if (!string.IsNullOrWhiteSpace(item.FaviconUrl))
                {
                    sb.AppendLine($"        <img class=\"favicon\" src=\"{System.Net.WebUtility.HtmlEncode(item.FaviconUrl)}\" alt=\"favicon\" onerror=\"this.style.display='none'\" title=\"{System.Net.WebUtility.HtmlEncode(item.Address)}\" />");
                }
                else
                {
                    sb.AppendLine("        <div class=\"favicon\"></div>");
                }

                sb.AppendLine("        <div class=\"history-content\">");
                sb.AppendLine($"            <a href=\"{System.Net.WebUtility.HtmlEncode(item.Address)}\" target=\"_blank\">{System.Net.WebUtility.HtmlEncode(item.Address)}</a>");
                sb.AppendLine($"            <div class=\"last-visited\">Last visited: {item.LastVisited:G}</div>");
                sb.AppendLine("        </div>");

                sb.AppendLine($"        <button class=\"delete-btn\" onclick=\"deleteItem('{id}')\">Delete</button>");
                sb.AppendLine("    </div>");
            }
        }

        // JavaScript using WebView2 messaging
        sb.AppendLine("<script>");
        sb.AppendLine("function deleteItem(id) {");
        sb.AppendLine("    window.chrome.webview.postMessage({ Action: 'delete', Ids: [id] });");
        sb.AppendLine("}");
        sb.AppendLine("function deleteChecked() {");
        sb.AppendLine("    const checkedBoxes = document.querySelectorAll('.item-checkbox:checked');");
        sb.AppendLine("    const ids = Array.from(checkedBoxes).map(cb => cb.closest('.history-item').dataset.id);");
        sb.AppendLine("    window.chrome.webview.postMessage({ Action: 'delete', Ids: ids });");
        sb.AppendLine("}");
        sb.AppendLine("function toggleAll(source) {");
        sb.AppendLine("    document.querySelectorAll('.item-checkbox').forEach(cb => cb.checked = source.checked);");
        sb.AppendLine("}");
        sb.AppendLine("</script>");

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}

//public static class HtmlGenerator
//{
//public static string GenerateHistoryHtml(ObservableCollection<HistoryItem> historyItems)
//{
//    if (historyItems == null) throw new ArgumentNullException(nameof(historyItems));

//    var sb = new StringBuilder();

//    // Start of HTML document
//    sb.AppendLine("<!DOCTYPE html>");
//    sb.AppendLine("<html lang=\"en\">");
//    sb.AppendLine("<head>");
//    sb.AppendLine("    <meta charset=\"UTF-8\">");
//    sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
//    sb.AppendLine("    <title>History</title>");
//    sb.AppendLine("    <style>");
//    sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; background-color: #f9f9f9; }");
//    sb.AppendLine("        h2 { margin-top: 30px; color: #333; border-bottom: 1px solid #ccc; padding-bottom: 5px; }");
//    sb.AppendLine("        .history-item { display: flex; align-items: center; padding: 10px; margin-bottom: 10px; background-color: #fff; border-radius: 5px; box-shadow: 0 0 5px rgba(0,0,0,0.1); }");
//    sb.AppendLine("        .favicon { width: 16px; height: 16px; margin-right: 10px; }");
//    sb.AppendLine("        .history-content { display: flex; flex-direction: column; }");
//    sb.AppendLine("        .history-item a { font-size: 16px; font-weight: bold; color: #007acc; text-decoration: none; }");
//    sb.AppendLine("        .history-item a:hover { text-decoration: underline; }");
//    sb.AppendLine("        .last-visited { font-size: 12px; color: #555; margin-top: 2px; }");
//    sb.AppendLine("    </style>");
//    sb.AppendLine("</head>");
//    sb.AppendLine("<body>");
//    sb.AppendLine("    <h1>Browsing History</h1>");

//    // Sort by most recent first
//    var sortedItems = historyItems.OrderByDescending(h => h.LastVisited);

//    // Group by day
//    var grouped = sortedItems.GroupBy(h => h.LastVisited.Date);

//    foreach (var group in grouped)
//    {
//        sb.AppendLine($"    <h2>{group.Key:dddd, MMMM d, yyyy}</h2>");
//        foreach (var item in group)
//        {
//            sb.AppendLine("    <div class=\"history-item\">");

//            // Favicon, optional
//            if (!string.IsNullOrWhiteSpace(item.FaviconUrl))
//            {
//                sb.AppendLine($"        <img class=\"favicon\" src=\"{System.Net.WebUtility.HtmlEncode(item.FaviconUrl)}\" alt=\"favicon\" onerror=\"this.style.display='none'\" />");
//            }
//            else
//            {
//                sb.AppendLine($"        <div class=\"favicon\"></div>"); // placeholder space
//            }

//            // Address and last visited
//            sb.AppendLine("        <div class=\"history-content\">");
//            sb.AppendLine($"            <a href=\"{System.Net.WebUtility.HtmlEncode(item.Address)}\" target=\"_blank\">{System.Net.WebUtility.HtmlEncode(item.Address)}</a>");
//            sb.AppendLine($"            <div class=\"last-visited\">Last visited: {item.LastVisited:G}</div>");
//            sb.AppendLine("        </div>");

//            sb.AppendLine("    </div>");
//        }
//    }

//    // End of HTML document
//    sb.AppendLine("</body>");
//    sb.AppendLine("</html>");

//    return sb.ToString();
//}


//}
