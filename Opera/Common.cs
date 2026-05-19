using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Opera
{
    public static class Common
    {
        public static string AppFolder = System.IO.Path.GetDirectoryName(Environment.ProcessPath);

        public static void DebugWriteLine(string msg)
        {
            Debug.WriteLine($"Debug: {msg}");
        }
    }
}
