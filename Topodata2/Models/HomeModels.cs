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
        [MustContainString(ErrorMessage = "Debe ser el link embed")]
        public string UrlVideo { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class MustContainString : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value.ToString().Contains("/embed/"))
            {
                return true;
            }
                return false;
        }
    }
}