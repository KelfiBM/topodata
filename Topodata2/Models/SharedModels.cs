using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Topodata2.Models
{
    public class BTableModel
    {
        public string IdTable { get; set; }
        public string UrlDeleteRecord { get; set; }
        public string IdOperationStatus { get; set; }
        public string UrlDataTable { get; set; }
        public bool DetailFormatter { get; set; }
        public Dictionary<string,string> ColumData { get; set; }
    }
}