using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Topodata2.Models.User;
using Topodata2.resources.Strings;

namespace Topodata2.Models.Mail
{
    public class MailManager
    {


        private static string GetYouTubeImage(string videoUrl)
        {
            var mInd = videoUrl.IndexOf("/embed/", StringComparison.Ordinal);
            if (mInd == -1) return "";

            var strVideoCode = videoUrl.Substring(videoUrl.IndexOf("/embed/", StringComparison.Ordinal) + 7);
            var ind = strVideoCode.IndexOf("?", StringComparison.Ordinal);
            strVideoCode = strVideoCode.Substring(0, ind == -1 ? strVideoCode.Length : ind);
            return "https://img.youtube.com/vi/" + strVideoCode + "/0.jpg";
        }

        private static MailMessage MakeMailMessage(string subject, string body, string to, AlternateView view = null,
            Attachment attachment = null)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(DomainSettings.EmailInfo),
                Subject = subject,
                Body = body,
                To = {to},
                IsBodyHtml = true
            };
            if (view != null)
            {
                mail.AlternateViews.Add(view);
            }
            if (attachment != null)
            {
                mail.Attachments.Add(attachment);
            }
            return mail;
        }

        private static bool SendMessage(string subject, string body, IEnumerable<string> to, AlternateView view = null,
            Attachment attachment = null)
        {
            var result = false;
            var smtp = new SmtpClient
            {
                Host = DomainSettings.HostEmail,
                Port = int.Parse(DomainSettings.HostPortSend),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(DomainSettings.EmailInfo, DomainSettings.EmailInfoPassword),
                EnableSsl = false
            };
            try
            {
                foreach (var recipent in to)
                {
                    smtp.Send(MakeMailMessage(subject, body, recipent, view, attachment));
                }

                result = true;
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                // ignored
            }
            finally
            {
                smtp.Dispose();
            }
            return result;
        }

        public static bool SendDeslinderRegistrationAdmin(DeslinderViewModel viewModel)
        {
            var result = false;
            try
            {
                string body;
                using (
                    var sr =
                        new StreamReader(
                            HttpContext.Current.Server.MapPath(Paths.EmailTemplateDeslindeUserRegisteredAdmin)))
                {
                    body = sr.ReadToEnd();
                }
                body = string.Format(body,
                    viewModel.Nombre,
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

                if (SendMessage(DomainSettings.EmailSubjectSendDeslindeRegistrationAdmin, body,
                    new[] {DomainSettings.EmailDeslinde}))
                {
                    result = true;
                }
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
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
                using (
                    var sr =
                        new StreamReader(
                            HttpContext.Current.Server.MapPath(Paths.EmailTemplateDeslindeUserRegisteredUser)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{0}", viewModel.Correo);

                var img1 = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgLogoDefault))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var img2 = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgDeslindePeople))
                {
                    ContentId = Guid.NewGuid().ToString()
                };

                body = body.Replace("{img1}", img1.ContentId);
                body = body.Replace("{img2}", img2.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(
                    body,
                    null,
                    DomainSettings.EmailAlternativeViewMediaType);
                view.LinkedResources.Add(img1);
                view.LinkedResources.Add(img2);

                if (SendMessage(DomainSettings.EmailSubjectSendDeslindeRegistrationUser,
                    body,
                    new[] {viewModel.Correo},
                    view))
                {
                    result = true;
                }
            }
                // ReSharper disable once UnusedVariable
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
                using (
                    var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateNewDocumentAdded)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{title1}", viewModel.Nombre);
                body = body.Replace("{categoria1}", viewModel.SubCategoria);
                body = body.Replace("{descripcion1}", viewModel.Descripcion);
                body = body.Replace("{urlDocument}", "topodata.com/Services/Document/" + viewModel.Id);

                var img0 = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgLogoDefault))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var img1 = new LinkedResource(HttpContext.Current.Server.MapPath("~" + viewModel.ImagePath))
                {
                    ContentId = Guid.NewGuid().ToString()
                };

                body = body.Replace("{img0}", img0.ContentId);
                body = body.Replace("{img1}", img1.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null,
                    DomainSettings.EmailAlternativeViewMediaType);
                view.LinkedResources.Add(img0);
                view.LinkedResources.Add(img1);

                var informedUsers = new List<UserModel>();

                if (UserManager.ExistsInformedUsers())
                {
                    informedUsers.AddRange(UserManager.GetAllInformedUsers());
                }
                if (UserManager.ExistsSuscribedInformed())
                {
                    informedUsers.AddRange(UserManager.GetSuscribedInformed());
                }
                if (informedUsers.Count > 0)
                {
                    var emails = informedUsers.Select(informedUser => informedUser.Email).ToList();
                    SendMessage(DomainSettings.EmailSubjectSendNewDocumentMessage,
                        body,
                        emails,
                        view,
                        new Attachment(HttpContext.Current.Server.MapPath(Paths.ImgVolanteTopogis)));
                    result = true;
                }
            }
                // ReSharper disable once UnusedVariable
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
                using (
                    var sr =
                        new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateRegistrationDoneUser)))
                {
                    body = sr.ReadToEnd();
                }
                var img0 = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgLogoDefault))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var img1 = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgEmailArchitect))
                {
                    ContentId = Guid.NewGuid().ToString()
                };

                body = body.Replace("{img0}", img0.ContentId);
                body = body.Replace("{img1}", img1.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(
                    body,
                    null,
                    DomainSettings.EmailAlternativeViewMediaType);
                view.LinkedResources.Add(img0);
                view.LinkedResources.Add(img1);

                SendMessage(DomainSettings.EmailSubjectSendRegistrationDone,
                    body,
                    new[] {model.Email},
                    view,
                    new Attachment(HttpContext.Current.Server.MapPath(Paths.ImgVolanteTopogis)));

                result = true;
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
            return result;
        }

        public static bool SendRegistrationDoneAdmin(UserModel model)
        {
            var result = false;
            try
            {
                string body;
                using (
                    var sr =
                        new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateRegistrationDoneUserAdmin))
                    )
                {
                    body = sr.ReadToEnd();
                }
                body = string.Format(body,
                    model.Name,
                    model.LastName,
                    model.Email,
                    model.UserName,
                    model.RegDate.Date
                    );

                SendMessage(DomainSettings.EmailSubjectSendRegistrationDoneAdmin,
                    body,
                    new[] {DomainSettings.EmailRegistrados});
                result = true;
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }

            return result;
        }

        public static bool SendSubscribeDone(string email)
        {
            var result = false;
            try
            {
                string body;
                using (
                    var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateSubscribedDoneMin)))
                {
                    body = sr.ReadToEnd();
                }
                var imgLogo = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgLogoDefault))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var img1 = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgEmailSubscribeDone))
                {
                    ContentId = Guid.NewGuid().ToString()
                };

                body = body.Replace("{imgLogo}", imgLogo.ContentId);
                body = body.Replace("{img1}", img1.ContentId);
                body = body.Replace("{url1}", DomainSettings.HostHome);
                var view = AlternateView.CreateAlternateViewFromString(body, null,
                    DomainSettings.EmailAlternativeViewMediaType);
                view.LinkedResources.Add(imgLogo);
                view.LinkedResources.Add(img1);

                SendMessage(DomainSettings.EmailSubjectSendSubscribeDone,
                    body,
                    new[] {email},
                    view,
                    new Attachment(HttpContext.Current.Server.MapPath(Paths.ImgVolanteTopogis)));
                result = true;
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
            return result;
        }

        public static bool SendHomeVideoUpload(HomeSliderViewModel model)
        {
            var result = false;
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateHomeVideoAdded)))
                {
                    body = sr.ReadToEnd();
                }

                var logo = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgLogoDefault))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                /*var imgVideo = new LinkedResource()
                {
                    ContentId = Guid.NewGuid().ToString()
                };*/
                var iconFacebook = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.imgHomeVideoIconFacebook))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var iconTwitter = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.imgHomeVideoIconTwitter))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var iconYoutube = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.imgHomeVideoIconYoutube))
                {
                    ContentId = Guid.NewGuid().ToString()
                };

                body = body.Replace("{logo}", logo.ContentId);
                body = body.Replace("{imgVideo}", GetYouTubeImage(model.UrlVideo));
                body = body.Replace("{iconFacebook}", iconFacebook.ContentId);
                body = body.Replace("{iconTwitter}", iconTwitter.ContentId);
                body = body.Replace("{iconYoutube}", iconYoutube.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null,
                    DomainSettings.EmailAlternativeViewMediaType);
                view.LinkedResources.Add(logo);
                view.LinkedResources.Add(iconFacebook);
                view.LinkedResources.Add(iconTwitter);
                view.LinkedResources.Add(iconYoutube);

                var informedUsers = new List<UserModel>();

                if (UserManager.ExistsInformedUsers())
                {
                    informedUsers.AddRange(UserManager.GetAllInformedUsers());
                }
                if (UserManager.ExistsSuscribedInformed())
                {
                    informedUsers.AddRange(UserManager.GetSuscribedInformed());
                }
                if (informedUsers.Count > 0)
                {
                    var emails = informedUsers.Select(i => i.Email).ToList();
                    SendMessage(DomainSettings.EmailSubjectSendHomeVideoUpload,
                        body,
                        emails,
                        view);
                    result = true;
                }
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
            return result;
        }

        public static bool SendContactUs(ContactUsModel model)
        {
            var result = false;
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateContactUsAdmin)))
                {
                    body = sr.ReadToEnd();
                }
                body = string.Format(body,
                    model.Nombre,
                    model.Email,
                    model.Mensaje
                    );

                if (SendMessage(DomainSettings.EmailSubjectSendContactUs, body,
                    new[] {DomainSettings.EmailContact}))
                {
                    result = true;
                }
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
            return result;
        }
    }
}