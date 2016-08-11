using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Topodata2.Models.Service;
using Topodata2.Models.User;

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
                    viewModel.Municipio,
                    viewModel.Provincia,
                    viewModel.Area,
                    viewModel.RegDate.Date
                    );
                MailMessage mail = new MailMessage();
                mail.To.Add("deslinde@topodata.com");
                mail.From = new MailAddress("info@topodata.com");
                mail.Subject = "Usuario Deslinde Registrado";
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
                img2.ContentId = Guid.NewGuid().ToString();

                body = body.Replace("{img1}", img1.ContentId);
                body = body.Replace("{img2}", img2.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(img1);
                view.LinkedResources.Add(img2);


                MailMessage mail = new MailMessage();
                mail.To.Add(viewModel.Correo);
                mail.From = new MailAddress("info@topodata.com");
                mail.Subject = "Proceso de gestion de Título iniciado";
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

        public static bool SendNewDocumentMessage(DocumentModel viewModel)
        {
            var result = false;
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/AddedContent/NewDocumentAdded.html")))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{title1}", viewModel.Nombre);
                body = body.Replace("{categoria1}", viewModel.SubCategoria);
                body = body.Replace("{descripcion1}", viewModel.Descripcion);
                body = body.Replace("{urlDocument}", "topodata.com/Services/Document/" + viewModel.Id);

                var img0 = new LinkedResource(HttpContext.Current.Server.MapPath("~/resources/img/logoDefault.png"))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var img1 = new LinkedResource(HttpContext.Current.Server.MapPath("~" + viewModel.ImagePath))
                {
                    ContentId = Guid.NewGuid().ToString()
                };

                body = body.Replace("{img0}", img0.ContentId);
                body = body.Replace("{img1}", img1.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(img0);
                view.LinkedResources.Add(img1);


                var mail = new MailMessage
                {
                    From = new MailAddress("info@topodata.com"),
                    Subject = "NUEVO!! Documento Técnico",
                    Body = body,
                    IsBodyHtml = true
                };
                mail.AlternateViews.Add(view);
                if (UserManager.GetAllInformedUsers() != null)
                {
                    foreach (var informedUser in UserManager.GetAllInformedUsers())
                    {
                        mail.To.Add(informedUser.Email);
                    }
                }
                if (UserManager.GetSuscribedInformed() != null)
                {
                    foreach (var informedUser in UserManager.GetSuscribedInformed())
                    {
                        mail.To.Add(informedUser.Email);
                    }
                }
                var smtp = new SmtpClient
                {
                    Host = "mail.topodata.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("info@topodata.com", "Topo.1953"),
                    EnableSsl = false
                };
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

        public static bool SendRegistrationDone(UserModel model)
        {
            var result = false;
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/Registration/RegistrationDoneUser.html")))
                {
                    body = sr.ReadToEnd();
                }
                var img0 = new LinkedResource(HttpContext.Current.Server.MapPath("~/resources/img/logoDefault.png"))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var img1 = new LinkedResource(HttpContext.Current.Server.MapPath("~/resources/img/email/architect.jpg"))
                {
                    ContentId = Guid.NewGuid().ToString()
                };

                body = body.Replace("{img0}", img0.ContentId);
                body = body.Replace("{img1}", img1.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(img0);
                view.LinkedResources.Add(img1);


                var mail = new MailMessage
                {
                    From = new MailAddress("info@topodata.com"),
                    Subject = "Introducete a TOPODATA",
                    Body = body,
                    IsBodyHtml = true
                };
                mail.AlternateViews.Add(view);
                mail.To.Add(model.Email);
                var smtp = new SmtpClient
                {
                    Host = "mail.topodata.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("info@topodata.com", "Topo.1953"),
                    EnableSsl = false
                };
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