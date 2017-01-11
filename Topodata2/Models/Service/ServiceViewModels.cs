using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Topodata2.Classes;
using Topodata2.resources.Strings;

namespace Topodata2.Models.Service
{
    public class DocumentViewModel : ViewModelAbstract
    {
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [Display(Name = "Nombre del documento")]
        [StringLength(500,
            MinimumLength = 5,
            ErrorMessage = "El nombre del documento debe tener maximo 500 caracteres y minimo 5")]
        public string Nombre { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripcion")]
        [StringLength(4000,
            MinimumLength = 5,
            ErrorMessage = "La descripcion debe tener maximo 4000 caracteres y minimo 5")]
        public string DescripcionHtml { get; set; }

        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [DataType(DataType.Url)]
        [Display(Name = "Url del Documento")]
        public string Url { get; set; }

        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [Display(Name = "Sector")]
        public int IdSubCategoria { get; set; }

        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [Display(Name = "Grupo")]
        public int IdContenido { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Imagen")]
        public HttpPostedFileBase ImageUpload { get; set; }

        public string ImagePath { get; set; }

        public string Descripcion { get; set; }
    }

    public class SubCategorieViewModel : ViewModelAbstract
    {
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [Display(Name = "Sector")]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [Display(Name = "Nuevo nombre")]
        public string Descripcion { get; set; }
    }
}