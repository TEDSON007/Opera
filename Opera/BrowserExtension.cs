using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Opera;

public class BrowserExtension
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string InjectJs { get; set; }
    public string InjectCss { get; set; }
    public List<string> Matches { get; set; }
    public string FolderPath { get; set; }

    public static List<BrowserExtension> LoadExtensions()
    {
        List<BrowserExtension> result = new();

        string extensionsFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions");

        if (!Directory.Exists(extensionsFolder))
            Directory.CreateDirectory(extensionsFolder);

        foreach (string dir in Directory.GetDirectories(extensionsFolder))
        {
            string manifestPath = Path.Combine(dir, "manifest.json");

            if (!File.Exists(manifestPath))
                continue;
            string jsPath = Path.Combine(dir, "content.js");
            
            if (!File.Exists(jsPath))
                continue;

            string json = File.ReadAllText(manifestPath);
            //string js = File.ReadAllText(jsPath);

            BrowserExtension ext =
                JsonSerializer.Deserialize<BrowserExtension>(json);

            ext.FolderPath = dir;
            ext.InjectJs = jsPath;


            result.Add(ext);
        }

        return result;
    }
}