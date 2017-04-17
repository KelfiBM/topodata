using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Topodata2.ViewModels
{
    public class SubscribeViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "Correo invalido")]
        [Remote("emailExistsSubscribe", "User", HttpMethod = "POST", ErrorMessage = "Este correo ya esta suscrito")]
        public string Email { get; set; }

    }
}