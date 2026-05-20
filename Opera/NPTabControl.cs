using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;


namespace Opera;

public class NPTabControl : TabControl
{
    #region Fields
    #endregion

    #region Properties
    public ObservableCollection<NPTab> NPTabs { get; set; } = [];
    #endregion

    #region Constructors
    public NPTabControl()
    {
        ItemsSource = NPTabs;
    }

    #endregion

    #region Nethods
    public NPTab CreateTab(string? uri = null)
    {
        string startPagePath = Path.Combine(Common.AppFolder, "StartPage.html");
        var nPTab = new NPTab(uri ?? startPagePath)
        {
            Header = uri ?? "Start",
        };
        
        return nPTab;
    }
    #endregion
}
