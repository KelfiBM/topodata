using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Topodata2.Models.Mail
{
    public class MailManager
    {
        public static bool SendDeslinderRegistrationAdmin(DeslinderViewModel viewModel)
        {
            var result = false;
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/Deslinder/DeslinderUserRegisteredAdmin.html")))
                {
                    body = sr.ReadToEnd();
                }
                body = string.Format(body,viewModel.Nombre, 
                    viewModel.Apellido, 
                    viewModel.Telefono,
                    viewModel.Movil,
                    viewModel.Correo,
                    viewModel.Ubicacion,
                    viewModel.NoMatrical,
                    viewModel.NoParsela,
                    viewModel.NoDistrito,
                    viewModel.Area
                    );
                MailMessage mail = new MailMessage();
                mail.To.Add("deslinder@topodata.com");
                mail.From = new MailAddress("info@topodata.com");
                mail.Subject = "Usuario Deslinder Registrado";
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.topodata.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("info@topodata.com", "Topo.1953");
                smtp.EnableSsl = false;
                smtp.Send(mail);
                result = true;
            }
            catch (Exception exception)
            {
                //Ignore
            }
            return result;
        }

        public static bool SendDeslinderRegistrationUser(DeslinderViewModel viewModel)
        {
            var result = false;
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/Deslinder/DeslinderUserRegisteredUser.html")))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{0}", viewModel.Correo);

                var img1 = new LinkedResource(HttpContext.Current.Server.MapPath("~/EmailTemplates/Deslinder/images/logoDefault.png"));
                img1.ContentId = Guid.NewGuid().ToString();
                var img2 = new LinkedResource(HttpContext.Current.Server.MapPath("~/EmailTemplates/Deslinder/images/81.jpg"));
                img1.ContentId = Guid.NewGuid().ToString();

                body = body.Replace("{img1}", img1.ContentId);
                body = body.Replace("{img2}", img2.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(img1);
                view.LinkedResources.Add(img2);


                MailMessage mail = new MailMessage();
                mail.To.Add(viewModel.Correo);
                mail.From = new MailAddress("info@topodata.com");
                mail.Subject = "Proceso de gestion de titulo iniciado";
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.AlternateViews.Add(view);

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.topodata.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("info@topodata.com", "Topo.1953");
                smtp.EnableSsl = false;
                smtp.Send(mail);
                smtp.Dispose();
                result = true;
            }
            catch (Exception e)
            {
                //Ignore
            }
            return result;
        }
    }
}