using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Topodata2.Models.Service
{
    public class DocumentViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Nombre del documento")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Url)]
        [Display(Name = "Url del Documento")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Sector")]
        public int IdSubCategoria { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Grupo")]
        public int IdContenido { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Imagen")]
        public HttpPostedFileBase ImageUpload { get; set; }

        public string ImagePath { get; set; }
    }
}