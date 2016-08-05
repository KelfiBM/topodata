using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;

namespace Topodata2.Models
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Nombre")]
        [DataType(DataType.Text)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Correo Electronico")]
        [EmailAddress(ErrorMessage = "Correo invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Favor escriba algun mensaje")]
        [Display(Name = "Mensaje")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; }

        public bool SendMessage(ContactUsViewModel contactUs)
        {
            try
            {
                const string from = "contact@topodata.com";
                const string to = "info@topodata.com";
                const string subject = "Correo de un visitante";
                const string pass = "Topo.1953";

                string body = "<b>De parte de:</b> " + contactUs.Nombre + "<br/>";
                body += "<b>Email:</b> " + contactUs.Email + "<br/>";
                body += "<b>Mensaje:</b> <br/>" + contactUs.Mensaje + "<br/>";

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from,"Contacto Topodata");
                mailMessage.To.Add(new MailAddress(to));
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Credentials = new NetworkCredential(from,pass);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }

    public class HomeTextPrincipalViewModel
    {
        [Required(ErrorMessage = "No puede estar en blanco")]
        [Display(Name = "Agrimensura")]
        [DataType(DataType.MultilineText)]
        [StringLength(200,ErrorMessage = "Debe tener un maximo de 200 caracteres")]
        public string Agrimensura { get; set; }

        [Required(ErrorMessage = "No puede estar en blanco")]
        [Display(Name = "Estudio del Suelo")]
        [DataType(DataType.MultilineText)]
        [StringLength(200, ErrorMessage = "Debe tener un maximo de 200 caracteres")]
        public string EstudioSuelo { get; set; }

        [Required(ErrorMessage = "No puede estar en blanco")]
        [Display(Name = "Diseño")]
        [DataType(DataType.MultilineText)]
        [StringLength(200, ErrorMessage = "Debe tener un maximo de 200 caracteres")]
        public string Diseno { get; set; }

        [Required(ErrorMessage = "No puede estar en blanco")]
        [Display(Name = "Ingenieria")]
        [DataType(DataType.MultilineText)]
        [StringLength(200, ErrorMessage = "Debe tener un maximo de 200 caracteres")]
        public string Ingenieria { get; set; }

        public int IdHomeText { get; set; }

        public DateTime RegDate { get; set; }
    }

    public class HomeSliderViewModel
    {
        public int IdVideoHome { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Link (embed) del video en youtube")]
        [DataType(DataType.Url)]
        [MustContainEmbedYoutube(ErrorMessage = "Debe ser el link embed")]
        public string UrlVideo { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class MustContainEmbedYoutube : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value.ToString().Contains("/embed/");
        }
    }

    public class OurTeamViewModel
    {
        public int IdOurTeam { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Nombre")]
        [DataType(DataType.Text)]
        [StringLength(50,ErrorMessage = "Debe tener maximo 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Cargo que ocupa")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Debe tener maximo 50 caracteres")]
        public string Cargo { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Correo Electronico")]
        [EmailAddress(ErrorMessage = "Correo invalido")]
        [StringLength(50, ErrorMessage = "Debe tener maximo 50 caracteres")]
        public string Email { get; set; }

        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Debe agregar una foto")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }
    }

    public class FlipboardViewModel
    {
        public int IdFlipboard { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Link de la revista")]
        [DataType(DataType.Url)]
        [MustContainFlipboard(ErrorMessage = "Debe ser un link de una revista de topodata en Flipboard")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        public string Name { get; set; }

        public DateTime RegDate { get; set; }
    }

    public class MustContainFlipboard : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value.ToString().Contains("flipboard.com/@topodata/");
        }
    }

    public class DeslinderViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Nombres")]
        [DataType(DataType.Text)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Apellidos")]
        [DataType(DataType.Text)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Direccion/Ubicacion exacta del terreno (Indique referencias)")]
        [DataType(DataType.Text)]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Telefono")]
        [DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }

        [Display(Name = "Movil")]
        [DataType(DataType.PhoneNumber)]
        public string Movil { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Correo Electronico")]
        [EmailAddress(ErrorMessage = "Correo invalido")]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "No. Matrícula del Título")]
        [DataType(DataType.Text)]
        public string NoMatrical { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "No. de Parcela (Del Terreno)")]
        [DataType(DataType.Text)]
        public string NoParsela { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "No. de Distrito Catastral")]
        [DataType(DataType.Text)]
        public string NoDistrito { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Municipio")]
        [DataType(DataType.Text)]
        public string Municipio { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Provincia")]
        [DataType(DataType.Text)]
        public string Provincia { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Area en Metros Cuadrados")]
        [DataType(DataType.Text)]
        public string Area { get; set; }

        public DateTime RegDate { get; set; }
    }
}