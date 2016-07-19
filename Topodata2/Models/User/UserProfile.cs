using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Topodata2.Models.User
{
    public class UserProfileSettingsNotificationViewModel
    {
        
        [Display(Name = "Notificarme por correo sobre los documentos nuevos en TOPODATA")]
        public bool newDocuments { get; set; }
    }
}