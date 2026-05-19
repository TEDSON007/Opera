using System.Windows;
using System.Windows.Controls;
using HtmlAgilityPack;

namespace Opera;

public class NPTab : TabItem
{
    #region Properties
    private string Title { get; set; }
    #endregion

    #region Events

    #endregion

    #region Constructors
    public NPTab()
    {
        
    }
    #endregion

    #region Methods
    public void UpdateTitle(string address)
    {
        string host = new Uri(address).Host;
        //string pageTitle = 
    }
    #endregion
}
