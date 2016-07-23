using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace Topodata2.Models.User
{
    public class UserProfileSettings
    {
        

        
    }
    public class UserProfileSettingsNotificationViewModel
    {
        [Display(Name = "Notificarme por correo sobre los documentos nuevos en TOPODATA")]
        public bool NewDocuments { get; set; }
    }
}