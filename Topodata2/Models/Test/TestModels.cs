using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Topodata2.Models.Test
{
    public class SendRegistrationDone
    {
        [Required]
        [Display(Name = "A partir de la fecha")]
        public string Fecha { get; set; }
    }
}