using System.Collections.Generic;

namespace Topodata2.Models.Shared
{
    public class BTableModel
    {
        public string IdTable { get; set; }
        /*public string UrlDeleteRecord { get; set; }*/
        /*public string IdOperationStatus { get; set; }*/
        /*public string UrlDataTable { get; set; }*/
        public bool DetailFormatter { get; set; }
        public Dictionary<string,string> ColumData { get; set; }
        public Dictionary<string,string> CustomColumData { get; set; }
        public int IndexActionType { get; set; }
    }
    public struct ThreeValuesString
    {
        public string Key;
        public string Value1;
        public string Value2;
    }
}