using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Opera;

public class History
{
    // TODO Untested
    static readonly string savePath = Path.Combine(Common.AppFolder, "history.json");

    public ObservableCollection<HistoryItem>? Former { get; set; }

    public void Remove(string address)
    {
        Former?.Remove(Former.FirstOrDefault(hi => hi.Address == address));
    }

    public void Add(string address)
    {
        HistoryItem hi = new()
        {
            Address = address,
            LastVisited = DateTime.Now
        };
        Former?.Add(hi);
    }

    public void Save()
    {
        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        string js = System.Text.Json.JsonSerializer.Serialize(this, options);
        File.WriteAllText(savePath, js);
    }

    public static History Load()
    {
        if(!File.Exists(savePath))
        {
            return new History();
        }
        string js = File.ReadAllText(savePath);
        return System.Text.Json.JsonSerializer.Deserialize<History>(js);
    }
}

public class HistoryItem
{
    public string Address { get; set; }
    public DateTime LastVisited { get; set; }

    public void UpdateVisited(DateTime lastVisited)
    {
        LastVisited = lastVisited;
    }

    public override string ToString()
    {
        return Address;
    }
}


