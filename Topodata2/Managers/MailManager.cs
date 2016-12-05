using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using SparkPost;
using Topodata2.Classes;
using Topodata2.Models;
using Topodata2.Models.Home;
using Topodata2.Models.Mail;
using Topodata2.Models.User;
using Topodata2.resources.Strings;

namespace Topodata2.Managers
{
    public class MailManager
    {
        //private const int Clientcount = 15;
        //private readonly SmtpClient[] _smtpClients = new SmtpClient[Clientcount + 1];
        //private CancellationTokenSource _cancelToken;

        /*public MailManager()
        {
            SetupSmtpClients();
        }*/

        /*~MailManager()
        {
            DisposeSmtpClients();
        }*/

        /*private void SetupSmtpClients()
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
        }*/

        /* private void DisposeSmtpClients()
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
        }*/

        /* public void CancelEmailRun()
        {
            _cancelToken.Cancel();
        }*/

        /*private void Send(MailMessage mailMessage)
        {
            using (mailMessage)
            {
                /#1#*var gotlocked = false;
                while (!gotlocked)
                {
                    //Keep looping through all smtp client connections until one becomes available
                    for (var i = 0; i <= Clientcount; i++)
                    {
                        if (!Monitor.TryEnter(_smtpClients[i])) continue;
                        try
                        {
                            _smtpClients[i].Send(mailMessage);
                            mailMessage.Dispose();
                        }
                        finally
                        {
                            Monitor.Exit(_smtpClients[i]);
                        }
                        gotlocked = true;
                        break;
                    }#2#
                    //Do this to make sure CPU doesn't ramp up to 100%
                    /*Thread.Sleep(100);#2#
                }#1#
            }
        }*/

/*
        private static MailMessage MakeMailMessage(MessageType messageType, string subject, string body, IEnumerable<string> to,
            AlternateView view = null,
            Attachment[] attachment = null)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(DomainSettings.EmailInfo),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                AlternateViews = { view },
            };
            switch (messageType)
            {
                case MessageType.Mass:
                    mail.To.Add(DomainSettings.EmailNo_reply);
                    foreach (var recipient in to)
                    {
                        mail.Bcc.Add(recipient);
                    }
                    break;
                case MessageType.Personal:
                    foreach (var recipient in to)
                    {
                        mail.To.Add(recipient);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
            
            if (attachment == null) return mail;
            foreach (var a in attachment)
            {
                mail.Attachments.Add(a);
            }
            return mail;
        }
*/

        private static Transmission MakeMailTransmission(MessageType messageType, string subject, string body,
            IEnumerable<string> to, IList<InlineImage> inlineImages = null, IList<Attachment> attachments = null)
        {
            var result = new Transmission
            {
                Content =
                {
                    Html = body,
                    Subject = subject,
                    From = {Email = DomainSettings.EmailInfo}
                }
            };
            if (inlineImages != null)
            {
                result.Content.InlineImages = inlineImages;
            }
            if (attachments != null)
            {
                result.Content.Attachments = attachments;
            }
            switch (messageType)
            {
                case MessageType.Mass:
                    result.Recipients.Add(new Recipient
                    {
                        Address = new Address {Email = DomainSettings.EmailNo_reply}
                    });
                    foreach (var recipient in to)
                    {
                        result.Recipients.Add(new Recipient
                        {
                            Address = new Address
                            {
                                Email = recipient,
                                HeaderTo = DomainSettings.EmailNo_reply
                            }
                        });
                    }
                    break;
                case MessageType.Personal:
                    foreach (var recipient in to)
                    {
                        result.Recipients.Add(new Recipient
                        {
                            Address = new Address
                            {
                                Email = recipient
                            }
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
            return result;
        }

        private static void Send(Transmission transmission)
        {
            var client = new Client(DomainSettings.SparkPostKey);
            client.CustomSettings.SendingMode = SendingModes.Async;
            client.Transmissions.Send(transmission);
            /*using (var client = new SmtpClient
            {
                Host = DomainSettings.HostEmail,
                Port = int.Parse(DomainSettings.HostPortSend),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(DomainSettings.EmailInfo, DomainSettings.EmailInfoPassword),
                EnableSsl = false
            })
            {
                client.Send(message);
                //Thread.Sleep(100);
            }*/
        }

        private static void SendMessage(MessageType messageType, string subject, string body, IEnumerable<string> to,
            IList<InlineImage> inlineImages = null, IList<Attachment> attachments = null)
        {
            /*var po = new ParallelOptions();
            //Create a cancellation token so you can cancel the task.
            _cancelToken = new CancellationTokenSource();
            po.CancellationToken = _cancelToken.Token;
            //Manage the MaxDegreeOfParallelism instead of .NET Managing this. We dont need 500 threads spawning for this.
            po.MaxDegreeOfParallelism = Environment.ProcessorCount*2;*/
            Send(MakeMailTransmission(messageType,subject,body,to,inlineImages,attachments));
        }

        private static void SendContactUs(ContactUsModel model, ContactUsViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new ContactUsModel
                    {
                        Email = viewModel.Email, Nombre = viewModel.Nombre, Mensaje = viewModel.Mensaje
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
                body = string.Format(body, model.Nombre, model.Email, model.Mensaje);
                SendMessage(MessageType.Personal, DomainSettings.EmailSubjectSendContactUs, body, new[] {DomainSettings.EmailContact});
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private static void SendHomeVideoUpload(HomeSliderVideo model, IReadOnlyCollection<UserModel> to,
            HomeSlideVideoViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new HomeSliderVideo {UrlVideo = viewModel.UrlVideo};
                }
                else return;
            }
            try
            {
                if (to.Count < 1) return;
                var images = new List<InlineImage>
                {
                    new InlineImage
                    {
                        Data = MailParameters.Logo76x72,
                        Type = MailParameters.ImageTypeGif,
                        Name = MailParameters.Logo76x72Name
                    },
                    new InlineImage
                    {
                        Data = MailParameters.IconFacebookHomeVideo,
                        Type = MailParameters.ImageTyePng,
                        Name = MailParameters.IconFacebookHomeVideoName
                    },
                    new InlineImage
                    {
                        Data = MailParameters.IconTwitterHomeVideo,
                        Type = MailParameters.ImageTyePng,
                        Name = MailParameters.IconTwitterHomeVideoName
                    },
                    new InlineImage
                    {
                        Data = MailParameters.IconYoutubeHomeVideo,
                        Type = MailParameters.ImageTyePng,
                        Name = MailParameters.IconYoutubeHomeVideoName
                    }
                };
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateHomeVideoAdded)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{imgVideo}", Youtube.GetImageFromId(model.UrlVideo));
                var emails = to.Select(i => i.Email).ToList();
                /*var ms = new MemoryStream();
                new FileStream(HttpContext.Current.Server.MapPath(Paths.ImgLogoDefault),FileMode.Open,FileAccess.Read).CopyTo(ms);
                byte[] data = ms.ToArray();*/
                SendMessage(MessageType.Mass, DomainSettings.EmailSubjectSendHomeVideoUpload, body, emails, images);
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                // ignored
            }
        }

        private static void SendSubscribeDone(string email)
        {
            try
            {
                var images = new List<InlineImage>
                {
                    new InlineImage
                    {
                        Data = MailParameters.Logo100x95,
                        Type = MailParameters.ImageTypeGif,
                        Name = MailParameters.Logo100x95Name
                    },
                    new InlineImage
                    {
                        Data = MailParameters.SubscribeDone,
                        Type = MailParameters.ImageTypeJpeg,
                        Name = MailParameters.SubscribeDoneName
                    }
                };
                var attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        Data = MailParameters.VolanteTopogis,
                        Type = MailParameters.ImageTypeJpeg,
                        Name = MailParameters.VolanteTopogisName
                    }
                };
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateSubscribedDoneMin)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{url1}", DomainSettings.HostHome);
                SendMessage(MessageType.Personal, DomainSettings.EmailSubjectSendSubscribeDone, body, new[] {email}, images, attachments);
            }
            catch
            {
                // ignored
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private static void SendRegistrationDoneAdmin(UserModel model, UserViewModel viewModel = null)
        {
            /*if (model == null)
            {
                if (viewModel != null)
                {
                    model = new UserModel
                    {
                    }
                }
                else
                {
                    return;
                }
            }*/

            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateRegistrationDoneUserAdmin)))
                {
                    body = sr.ReadToEnd();
                }
                body = string.Format(body, model.Name, model.LastName, model.Email, model.UserName, model.RegDate.Date);
                SendMessage(MessageType.Personal, DomainSettings.EmailSubjectSendRegistrationDoneAdmin, body, new[]
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

        // ReSharper disable once UnusedParameter.Local
        private static void SendRegistrationDoneUser(UserModel model, UserViewModel viewModel = null)
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
                var images = new List<InlineImage>
                {
                    new InlineImage
                    {
                        Data = MailParameters.Logo100x95,
                        Type = MailParameters.ImageTypeGif,
                        Name = MailParameters.Logo100x95Name
                    },
                    new InlineImage
                    {
                        Data = MailParameters.RegistrationDoneUser,
                        Type = MailParameters.ImageTypeJpeg,
                        Name = MailParameters.RegistrationDoneUserName
                    }
                };
                var attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        Data = MailParameters.VolanteTopogis,
                        Type = MailParameters.ImageTypeJpeg,
                        Name = MailParameters.VolanteTopogisName
                    }
                };
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateRegistrationDoneUser)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{username}", model.UserName);
                body = body.Replace("{contra}", model.Password);
                
                SendMessage(MessageType.Personal, DomainSettings.EmailSubjectSendRegistrationDone, body, new[] {model.Email}, images, attachments);
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
        }

        private static void SendNewDocumentMessage(DocumentModel model, IReadOnlyCollection<UserModel> to )
        {
            try
            {
                if (to.Count < 1) return;
                string imageDocument;
                using (var image = Image.FromFile(HttpContext.Current.Server.MapPath("~" + model.ImagePath)))
                {
                    using (var memory = new MemoryStream())
                    {
                        image.Save(memory,image.RawFormat);
                        imageDocument = Convert.ToBase64String(memory.ToArray(), Base64FormattingOptions.None);
                    }
                }
                var images = new List<InlineImage>
                {
                    new InlineImage
                    {
                        Data = MailParameters.Logo100x95,
                        Type = MailParameters.ImageTypeGif,
                        Name = MailParameters.Logo100x95Name
                    },
                    new InlineImage
                    {
                        Data = imageDocument,
                        Type = MailParameters.ImageTypeJpeg,
                        Name = MailParameters.NewDocumentName
                    }
                };
                var attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        Data = MailParameters.VolanteTopogis,
                        Type = MailParameters.ImageTypeJpeg,
                        Name = MailParameters.VolanteTopogisName
                    }
                };
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateNewDocumentAdded)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{title1}", model.Nombre);
                body = body.Replace("{categoria1}", model.SubCategoria);
                body = body.Replace("{descripcion1}", model.Descripcion);
                body = body.Replace("{urlDocument}", DomainSettings.UrlDocument + model.Id);
                
                var emails = to.Select(informedUser => informedUser.Email).ToList();
                SendMessage(MessageType.Mass, DomainSettings.EmailSubjectSendNewDocumentMessage, body, emails, images, attachments);
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
        }

        private static void SendDeslinderRegistrationUser(DeslinderModel model, DeslindeViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new DeslinderModel
                    {
                        Apellido = viewModel.Apellido, Area = viewModel.Area, Correo = viewModel.Correo, Movil = viewModel.Movil, Municipio = viewModel.Municipio, NoDistrito = viewModel.NoDistrito, NoMatrical = viewModel.NoMatrical, Nombre = viewModel.Nombre, NoParsela = viewModel.NoParsela, Provincia = viewModel.Provincia, Telefono = viewModel.Telefono, Ubicacion = viewModel.Ubicacion
                    };
                }
                else
                {
                    return;
                }
            }
            try
            {
                var images = new List<InlineImage>
                {
                    new InlineImage
                    {
                        Data = MailParameters.Logo100x95,
                        Type = MailParameters.ImageTypeGif,
                        Name = MailParameters.Logo100x95Name
                    },
                    new InlineImage
                    {
                        Data = MailParameters.DeslindePeople,
                        Type = MailParameters.ImageTypeJpeg,
                        Name = MailParameters.DeslindePeopleName
                    }
                };
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateDeslindeUserRegisteredUser)))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{modelCorreo}", model.Correo);
                SendMessage(MessageType.Personal, DomainSettings.EmailSubjectSendDeslindeRegistrationUser, body, new[] {model.Correo}, images);
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //ignored
            }
        }

        private static void SendDeslinderRegistrationAdmin(DeslinderModel model, DeslindeViewModel viewModel = null)
        {
            if (model == null)
            {
                if (viewModel != null)
                {
                    model = new DeslinderModel
                    {
                        Apellido = viewModel.Apellido, Area = viewModel.Area, Correo = viewModel.Correo, Movil = viewModel.Movil, Municipio = viewModel.Municipio, NoDistrito = viewModel.NoDistrito, NoMatrical = viewModel.NoMatrical, Nombre = viewModel.Nombre, NoParsela = viewModel.NoParsela, Provincia = viewModel.Provincia, Telefono = viewModel.Telefono, Ubicacion = viewModel.Ubicacion
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
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(Paths.EmailTemplateDeslindeUserRegisteredAdmin)))
                {
                    body = sr.ReadToEnd();
                }
                body = string.Format(body, model.Nombre, model.Apellido, model.Telefono, model.Movil, model.Correo, model.Ubicacion, model.NoMatrical, model.NoParsela, model.NoDistrito, model.Municipio, model.Provincia, model.Area, model.RegDate.Date);
                SendMessage(MessageType.Personal, DomainSettings.EmailSubjectSendDeslindeRegistrationAdmin, body, new[]
                {
                    DomainSettings.EmailDeslinde
                });
            }
                // ReSharper disable once UnusedVariable
            catch (Exception e)
            {
                //Ignore
            }
        }

        private static void SendMailThread(MailType mailType, object model)
        {
            switch (mailType)
            {
                case MailType.DeslinderRegistrationAdmin:
                    var dRegistrationAdmin = model as DeslinderModel;
                    var dRegistrationAdminView = model as DeslindeViewModel;
                    if (dRegistrationAdmin != null || dRegistrationAdminView != null)
                    {
                        SendDeslinderRegistrationAdmin(dRegistrationAdmin, dRegistrationAdminView);
                    }
                    break;
                case MailType.DeslinderRegistrationUser:
                    var dRegistrationUser = model as DeslinderModel;
                    var dRegistrationUserView = model as DeslindeViewModel;
                    if (dRegistrationUser != null || dRegistrationUserView != null)
                    {
                        SendDeslinderRegistrationUser(dRegistrationUser, dRegistrationUserView);
                    }
                    break;
                case MailType.NewDocumentMessage:
                    var document = model as DocumentModel;
                    if (document != null)
                    {
                        var userList = UserManager.GetAllInformed();
                        SendNewDocumentMessage(document, userList);
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
                    var videoUploadView = model as HomeSlideVideoViewModel;
                    if (videoUpload != null || videoUploadView != null)
                    {
                        var userList = UserManager.GetAllInformed();
                        SendHomeVideoUpload(videoUpload, userList, videoUploadView);
                    }
                    break;
                case MailType.ContactUs:
                    var contactus = model as ContactUsModel;
                    var contactUsView = model as ContactUsViewModel;
                    if (contactus != null || contactUsView != null)
                    {
                        SendContactUs(contactus, contactUsView);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mailType), mailType, null);
            }
        }

        public MailManager SendMail(MailType mailType, object model)
        {
            SendMailThread(mailType, model);
            return this;
        }

        /* public void Dispose()
        {
            DisposeSmtpClients();
        }*/
    }
}