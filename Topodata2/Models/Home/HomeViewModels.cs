using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Topodata2.Classes;
using Topodata2.resources.Strings;

namespace Topodata2.Models.Home
{
    public class HomeSliderImageSeasonViewModel : ViewModelAbstract
    {
        public string ImagePath { get; set; }

        [DataType(DataType.Upload)]
        [Required(ErrorMessageResourceType = typeof(Displays), ErrorMessageResourceName = "MustPhoto")]
        public HttpPostedFileBase ImageUpload { get; set; }
    }

    public class HomeSlideVideoViewModel : ViewModelAbstract
    {
        [Display(ResourceType = typeof(Displays), Name = "HomeSliderVideoUrlVideo")]
        [DataType(DataType.Url)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string UrlVideo { get; set; }
    }

    public class DeslindeViewModel
    {
        [Display(ResourceType = typeof(Displays), Name = "Nombres")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Apellidos")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Apellido { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Ubicacion")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Ubicacion { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Telefono")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Telefono { get; set; }

        [Display(ResourceType = typeof (Displays), Name = "Movil")]
        [DataType(DataType.PhoneNumber)]
        public string Movil { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Correo")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [EmailAddress(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "CorreoInvalido",
            ErrorMessage = null)]
        public string Correo { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "NoMatrical")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string NoMatrical { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "NoParcela")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string NoParsela { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "NoDistrito")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string NoDistrito { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Municipio")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Municipio { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Provincia")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Provincia { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Area")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Area { get; set; }

        public DateTime RegDate { get; set; }
    }

    public class TextoHomeViewModel
    {
        [Display(ResourceType = typeof(Displays), Name = "Agrimensura")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "EmptyField")]
        [StringLength(200, ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Max200Char")]
        public string Agrimensura { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "EstudioSuelo")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "EmptyField")]
        [StringLength(200, ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Max200Char")]
        public string EstudioSuelo { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Diseno")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "EmptyField")]
        [StringLength(200, ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Max200Char")]
        public string Diseno { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Ingenieria")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "EmptyField")]
        [StringLength(200, ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Max200Char")]
        public string Ingenieria { get; set; }
    }

    public class OurTeamViewModel
    {
        [Display(ResourceType = typeof(Displays), Name = "Nombre")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [StringLength(50, ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Max50Char")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Cargo")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [StringLength(50, ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Max50Char")]
        public string Cargo { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Correo")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Max50Char")]
        [EmailAddress(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "CorreoInvalido",
            ErrorMessage = null)]
        public string Email { get; set; }

        public string ImagePath { get; set; }

        [DataType(DataType.Upload)]
        [Required(ErrorMessageResourceType = typeof (Displays), ErrorMessageResourceName = "MustPhoto")]
        public HttpPostedFileBase ImageUpload { get; set; }
    }

    public class FlipboardViewModel : ViewModelAbstract
    {
        [Display(ResourceType = typeof(Displays), Name = "UrlRevista")]
        [DataType(DataType.Url)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [MustContainFlipboard(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "MustTopodataFlipboard")]
        public string Url { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Nombre")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Name { get; set; }
    }

    public class ContactUsViewModel
    {
        [Display(ResourceType = typeof(Displays), Name = "Nombre")]
        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Correo")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "Requerido")]
        [EmailAddress(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "CorreoInvalido",
            ErrorMessage = null)]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Displays), Name = "Mensaje")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessageResourceType = typeof (Messages), ErrorMessageResourceName = "MustWrite")]
        public string Mensaje { get; set; }
    }

    public class MustContainFlipboard : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value.ToString().Contains("flipboard.com/@topodata/");
        }
    }

    public class AllFlipboardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string RegDate { get; set; }

    }
}