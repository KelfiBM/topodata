using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Topodata2.Classes;
using Topodata2.Models.Home;
using Topodata2.Models.User;
using Topodata2.resources.Strings;
using WebGrease;

namespace Topodata2.Models.Mail
{
    public class MailManager : IDisposable
    {
        private const int Clientcount = 15;
        private readonly SmtpClient[] _smtpClients = new SmtpClient[Clientcount + 1];
        private CancellationTokenSource _cancelToken;

        public MailManager()
        {
            SetupSmtpClients();
        }

        ~MailManager()
        {
            DisposeSmtpClients();
        }

        private void SetupSmtpClients()
        {
            for (var i = 0; i <= Clientcount; i++)
            {
                try
                {
                    var client = new SmtpClient
                    {
                        Host = DomainSettings.HostEmail,
                        Port = int.Parse(DomainSettings.HostPortSend),
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(DomainSettings.EmailInfo, DomainSettings.EmailInfoPassword),
                        EnableSsl = false
                    };
                    _smtpClients[i] = client;
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }

        private void DisposeSmtpClients()
        {
            for (var i = 0; i <= Clientcount; i++)
            {
                try
                {
                    _smtpClients[i].Dispose();
                }
                catch (Exception ex)
                {
                    //Log Exception
                }
            }
        }

        public void CancelEmailRun()
        {
            _cancelToken.Cancel();
        }

        private void Send(MailMessage mailMessage)
        {
            using (mailMessage)
            {
                var gotlocked = false;
                while (!gotlocked)
                {
                    //Keep looping through all smtp client connections until one becomes available
                    for (var i = 0; i <= Clientcount; i++)
                    {
                        if (!Monitor.TryEnter(_smtpClients[i])) continue;
                        try
                        {
                            _smtpClients[i].Send(mailMessage);
                        }
                        finally
                        {
                            Monitor.Exit(_smtpClients[i]);
                        }
                        gotlocked = true;
                        break;
                    }
                    //Do this to make sure CPU doesn't ramp up to 100%
                    Thread.Sleep(100);
                }
            }
        }

        private void SendMessage(string subject, string body, IEnumerable<string> to, AlternateView view = null,
            IEnumerable<Attachment> attachment = null)
        {
            var po = new ParallelOptions();
            //Create a cancellation token so you can cancel the task.
            _cancelToken = new CancellationTokenSource();
            po.CancellationToken = _cancelToken.Token;
            //Manage the MaxDegreeOfParallelism instead of .NET Managing this. We dont need 500 threads spawning for this.
            po.MaxDegreeOfParallelism = Environment.ProcessorCount*2;
            Parallel.ForEach(to, po, recipent =>
            {
                Send(MakeMailMessage(subject, body, recipent, view, attachment));
            });

        }

        private void SendContactUs(ContactUsModel model, ContactUsViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new ContactUsModel
                    {
                        Email = viewModel.Email,
                        Nombre = viewModel.Nombre,
                        Mensaje = viewModel.Mensaje
                    };
                }
                else
                {
                    return;
                }
            }
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
                SendMessage(DomainSettings.EmailSubjectSendContactUs,
                    body,
                    new[] {DomainSettings.EmailContact});
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void SendHomeVideoUpload(HomeSliderVideo model, HomeSliderVideoViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new HomeSliderVideo
                    {
                        UrlVideo = viewModel.UrlVideo,
                    };
                }
                else
                {
                    return;
                }
            }
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
                body = body.Replace("{imgVideo}", Youtube.GetImageFromId(model.UrlVideo));
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

                var informed = UserManager.GetAllInformedUsers();
                if (informed != null)
                {
                    informedUsers.AddRange(informed);
                }
                informed = UserManager.GetSuscribedInformed();
                if (informed != null)
                {
                    informedUsers.AddRange(informed);
                }
                if (informedUsers.Count <= 0) return;
                var emails = informedUsers.Select(i => i.Email).ToList();
                SendMessage(DomainSettings.EmailSubjectSendHomeVideoUpload,
                    body,
                    emails,
                    view);
            }
            catch
            {
                // ignored
            }
        }

        private void SendSubscribeDone(string email)
        {
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
                    new[]
                    {
                        new Attachment(HttpContext.Current.Server.MapPath(Paths.ImgVolanteTopogis))
                    });
            }
            catch
            {
                // ignored
            }
        }

        private void SendRegistrationDoneAdmin(UserModel model, UserViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new UserModel
                    {

                    };
                }
                else
                {
                    return;
                }
            }

            try
            {
                string body;
                using (var sr =
                    new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateRegistrationDoneUserAdmin)))
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
                    new[]
                    {
                        DomainSettings.EmailRegistrados
                    });
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendRegistrationDoneUser(UserModel model, UserViewModel viewModel = null)
        {
            /*if (model == null)
            {
                if (viewModel != null)
                {
                    model = new UserModel
                    {
                        Email = viewModel.
                        Apellido = viewModel.Apellido,
                        Area = viewModel.Area,
                        Correo = viewModel.Correo,
                        Movil = viewModel.Movil,
                        Municipio = viewModel.Municipio,
                        NoDistrito = viewModel.NoDistrito,
                        NoMatrical = viewModel.NoMatrical,
                        Nombre = viewModel.Nombre,
                        NoParsela = viewModel.NoParsela,
                        Provincia = viewModel.Provincia,
                        Telefono = viewModel.Telefono,
                        Ubicacion = viewModel.Ubicacion
                    };
                }
                else
                {
                    return;
                }

            }*/

            try
            {
                string body;
                using (var sr =
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
                body = body.Replace("{username}", model.UserName);
                body = body.Replace("{contra}", model.Password);

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
                    new[]
                    {
                        new Attachment(HttpContext.Current.Server.MapPath(Paths.ImgVolanteTopogis))
                    });

            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendNewDocumentMessage(DocumentModel model)
        {
            try
            {
                string body;
                using (var sr =
                    new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateNewDocumentAdded)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{title1}", model.Nombre);
                body = body.Replace("{categoria1}", model.SubCategoria);
                body = body.Replace("{descripcion1}", model.Descripcion);
                body = body.Replace("{urlDocument}", "topodata.com/Services/Document/" + model.Id);

                var img0 = new LinkedResource(HttpContext.Current.Server.MapPath(Paths.ImgLogoDefault))
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                var img1 = new LinkedResource(HttpContext.Current.Server.MapPath("~" + model.ImagePath))
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

                var informed = UserManager.GetAllInformedUsers();
                if (informed != null)
                {
                    informedUsers.AddRange(informed);
                }
                informed = UserManager.GetSuscribedInformed();
                if (informed != null)
                {
                    informedUsers.AddRange(informed);
                }
                if (informedUsers.Count <= 0) return;
                var emails = informedUsers.Select(informedUser => informedUser.Email).ToList();
                SendMessage(DomainSettings.EmailSubjectSendNewDocumentMessage,
                    body,
                    emails,
                    view,
                    new[]
                    {
                        new Attachment(HttpContext.Current.Server.MapPath(Paths.ImgVolanteTopogis))
                    });
            }
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendDeslinderRegistrationUser(DeslinderModel model, DeslinderViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new DeslinderModel
                    {
                        Apellido = viewModel.Apellido,
                        Area = viewModel.Area,
                        Correo = viewModel.Correo,
                        Movil = viewModel.Movil,
                        Municipio = viewModel.Municipio,
                        NoDistrito = viewModel.NoDistrito,
                        NoMatrical = viewModel.NoMatrical,
                        Nombre = viewModel.Nombre,
                        NoParsela = viewModel.NoParsela,
                        Provincia = viewModel.Provincia,
                        Telefono = viewModel.Telefono,
                        Ubicacion = viewModel.Ubicacion
                    };
                }
                else
                {
                    return;
                }
            }
            try
            {
                string body;
                using (var sr =
                    new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateDeslindeUserRegisteredUser)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{modelCorreo}", model.Correo);

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

                SendMessage(DomainSettings.EmailSubjectSendDeslindeRegistrationUser,
                    body,
                    new[] {model.Correo},
                    view);
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendDeslinderRegistrationAdmin(DeslinderModel model, DeslinderViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new DeslinderModel
                    {
                        Apellido = viewModel.Apellido,
                        Area = viewModel.Area,
                        Correo = viewModel.Correo,
                        Movil = viewModel.Movil,
                        Municipio = viewModel.Municipio,
                        NoDistrito = viewModel.NoDistrito,
                        NoMatrical = viewModel.NoMatrical,
                        Nombre = viewModel.Nombre,
                        NoParsela = viewModel.NoParsela,
                        Provincia = viewModel.Provincia,
                        Telefono = viewModel.Telefono,
                        Ubicacion = viewModel.Ubicacion
                    };
                }
                else
                {
                    return;
                }

            }
            try
            {
                string body;
                using (var sr =
                    new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateDeslindeUserRegisteredAdmin))
                    )
                {
                    body = sr.ReadToEnd();
                }
                body = string.Format(body,
                    model.Nombre,
                    model.Apellido,
                    model.Telefono,
                    model.Movil,
                    model.Correo,
                    model.Ubicacion,
                    model.NoMatrical,
                    model.NoParsela,
                    model.NoDistrito,
                    model.Municipio,
                    model.Provincia,
                    model.Area,
                    model.RegDate.Date
                    );
                SendMessage(DomainSettings.EmailSubjectSendDeslindeRegistrationAdmin,
                    body,
                    new[]
                    {
                        DomainSettings.EmailDeslinde
                    });
            }
            catch (Exception e)
            {
                //Ignore
            }
        }

        private static MailMessage MakeMailMessage(string subject, string body, string to, AlternateView view = null,
            IEnumerable<Attachment> attachment = null)
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
            if (attachment == null) return mail;
            foreach (var a in attachment)
            {
                mail.Attachments.Add(a);
            }
            return mail;
        }

        public MailManager SendMail(MailType mailType, object model)
        {
            switch (mailType)
            {
                case MailType.DeslinderRegistrationAdmin:
                    var dRegistrationAdmin = model as DeslinderModel;
                    var dRegistrationAdminView = model as DeslinderViewModel;
                    if (dRegistrationAdmin != null || dRegistrationAdminView != null)
                    {
                        SendDeslinderRegistrationAdmin(dRegistrationAdmin, dRegistrationAdminView);
                    }
                    break;
                case MailType.DeslinderRegistrationUser:
                    var dRegistrationUser = model as DeslinderModel;
                    var dRegistrationUserView = model as DeslinderViewModel;
                    if (dRegistrationUser != null || dRegistrationUserView != null)
                    {
                        SendDeslinderRegistrationUser(dRegistrationUser, dRegistrationUserView);
                    }
                    break;
                case MailType.NewDocumentMessage:
                    var document = model as DocumentModel;
                    if (document != null)
                    {
                        SendNewDocumentMessage(document);
                    }
                    break;
                case MailType.RegistrationDoneUser:
                    var doneUser = model as UserModel;
                    if (doneUser != null)
                    {
                        SendRegistrationDoneUser(doneUser);
                    }
                    break;
                case MailType.RegistrationDoneAdmin:
                    var doneAdmin = model as UserModel;
                    if (doneAdmin != null)
                    {
                        SendRegistrationDoneAdmin(doneAdmin);
                    }
                    break;
                case MailType.SubscribeDone:
                    var subscribeDone = model as string;
                    if (subscribeDone != null)
                    {
                        SendSubscribeDone(subscribeDone);
                    }
                    break;
                case MailType.HomeVideoUpload:
                    var videoUpload = model as HomeSliderVideo;
                    var videoUploadView = model as HomeSliderVideoViewModel;
                    if (videoUpload != null || videoUploadView != null)
                    {
                        SendHomeVideoUpload(videoUpload,videoUploadView);
                    }
                    break;
                case MailType.ContactUs:
                    var contactus = model as ContactUsModel;
                    var contactUsView = model as ContactUsViewModel;
                    if (contactus != null || contactUsView != null)
                    {
                        SendContactUs(contactus,contactUsView);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mailType), mailType, null);
            }
            return this;
        }

        public void Dispose()
        {
            DisposeSmtpClients();
        }


    }
}