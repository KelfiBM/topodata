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
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}