using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Topodata2.Classes;
using Topodata2.Models.Shared;

namespace Topodata2.Models.Administration
{
    public class AdministrationModel
    {
        public ActionType Action { get; set; }
        public bool UseTable { get; set; }
        public bool UseTextArea { get; set; }
        public string Title { get; set; }
        public string IdTabPrincipal { get; set; }
        public string IdHiddenRaw { get; set; }
        public string IdTable { get; set; }
        public List<string> IdsFormValidation { get; set; }
        public List<ThreeValuesString> Tabs { get; set; }
        public ViewModelAbstract ViewModel { get; set; }
        public bool UseDetailFormatter { get; set; }
    }
}