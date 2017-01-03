﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SparkPost;
using Topodata2.Classes;
using Topodata2.Models;
using Topodata2.Models.Home;
using Topodata2.Models.Mail;
using Topodata2.Models.User;
using Topodata2.resources.Strings;
using static System.Web.HttpContext;
using Attachment = System.Net.Mail.Attachment;
using File = System.IO.File;

namespace Topodata2.Managers
{
    public class MailManager : IDisposable
    {
        private const int Clientcount = 15;
        private readonly SmtpClient[] _smtpClients = new SmtpClient[Clientcount + 1];
        private readonly Client[] _transmissionClients = new Client[Clientcount + 1];
        private CancellationTokenSource _cancelToken;

        public MailManager()
        {
            //SetupSmtpClients();
            SetupTransmissionClients();
        }

        ~MailManager()
        {
            //DisposeSmtpClients();
            DisposeTransmissionClients();
        }

        private void SetupSmtpClients()
        {
            for (var i = 0; i <= Clientcount; i++)
            {
                try
                {
                    var client = new SmtpClient
                    {
                        Host = DomainSettings.HostSparkMail,
                        Port = int.Parse(DomainSettings.HostSparkPort),
                        UseDefaultCredentials = false,
                        Credentials =
                            new NetworkCredential(DomainSettings.HostSparkUsername, DomainSettings.KeySparkpost),
                        EnableSsl = true,
                    };
                    _smtpClients[i] = client;
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }

        private void SetupTransmissionClients()
        {
            for (var i = 0; i <= Clientcount; i++)
            {
                try
                {
                    var client = new Client(DomainSettings.KeySparkpost)
                    {
                          CustomSettings = { SendingMode = SendingModes.Sync}
                    };
                    _transmissionClients[i] = client;
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

        private void DisposeTransmissionClients()
        {
            for (var i = 0; i <= Clientcount; i++)
            {
                try
                {
                    //_transmissionClients[i].Dispose();
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
                    Thread.Sleep(1000);
                }
            }
        }

        private void Send(Transmission transmission)
        {
            var gotlocked = false;
            while (!gotlocked)
            {
                //Keep looping through all transmision client connections until one becomes available
                for (var i = 0; i <= Clientcount; i++)
                {
                    if (!Monitor.TryEnter(_transmissionClients[i])) continue;
                    try
                    {
                        _transmissionClients[i].Transmissions.Send(transmission);
                    }
                    finally
                    {
                        Monitor.Exit(_transmissionClients[i]);
                    }
                    gotlocked = true;
                    break;
                }
                //Do this to make sure CPU doesn't ramp up to 100%
                Thread.Sleep(500);
            }
        }

        private static void SendIndividual(MailMessage message)
        {
            var t = Task.Run(async () =>
            {
                using (var client = new SmtpClient
                {
                    Host = DomainSettings.HostSparkMail,
                    Port = int.Parse(DomainSettings.HostSparkPort),
                    UseDefaultCredentials = false,
                    Credentials =
                        new NetworkCredential(DomainSettings.HostSparkUsername, DomainSettings.KeySparkpost),
                    EnableSsl = true,
                })
                {
                    await client.SendMailAsync(message);
                }
            });
            t.Wait();
        }

        private void SendMessage(MessageType messageType, string subject, string body, IEnumerable<string> to,
            Attachment[] attachment = null)
        {
            var po = new ParallelOptions();
            //Create a cancellation token so you can cancel the task.
            _cancelToken = new CancellationTokenSource();
            po.CancellationToken = _cancelToken.Token;
            //Manage the MaxDegreeOfParallelism instead of .NET Managing this. We dont need 500 threads spawning for this.
            po.MaxDegreeOfParallelism = Environment.ProcessorCount*2;
            var enumerable = to as string[] ?? to.ToArray();
            Parallel.ForEach(enumerable, po, recipient =>
            {
                Send(MakeMailMessage(messageType, subject, body, new []{recipient}, attachment));
            });
        }

        private void SendTransmision(MessageType messageType, string subject, string body, IEnumerable<string> to,
            SparkPost.Attachment[] attachment = null)
        {
            var po = new ParallelOptions();
            //Create a cancellation token so you can cancel the task.
            _cancelToken = new CancellationTokenSource();
            po.CancellationToken = _cancelToken.Token;
            //Manage the MaxDegreeOfParallelism instead of .NET Managing this. We dont need 500 threads spawning for this.
            po.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;
            var enumerable = to as string[] ?? to.ToArray();
            Parallel.ForEach(enumerable, po, recipient =>
            {
                Send(MakeTransmission(messageType, subject, body, new[] { recipient }, attachment));
            });
        }

        public MailManager SendMail(MailType mailType, object model)
        {
            //var type = mailType is MailType ? (MailType) mailType : MailType.DeslinderRegistrationAdmin;
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
                        var userList = UserManager.GetAllInformedSeparated(10);
                        foreach (var users in userList)
                        {
                            SendNewDocumentMessage(document, users);
                        }
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
                        var userList = UserManager.GetAllInformedSeparated(10);
                        foreach (var users in userList)
                        {
                            SendHomeVideoUpload(videoUpload, users, videoUploadView);
                        }
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
            return this;
        }

        public void Dispose()
        {
            //DisposeSmtpClients();
            DisposeTransmissionClients();
        }
        //---------------------------------------------------------------------------------//
        private static string MakeBody(string pathTemplate, params object[] replaces)
        {
            string result;
            using (var sr = new StreamReader(Current.Server.MapPath(pathTemplate)))
            {
                result = sr.ReadToEnd();
            }
            if(replaces.Length > 0) result = string.Format(result,replaces);
            return result;
        }

        private static string MakeImageBase64(string imagePath)
        {
            var result = "";
            if (!File.Exists(imagePath)) return result;
            using (var image = Image.FromFile(imagePath))
            {
                using (var m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    result = Convert.ToBase64String(m.ToArray());
                }
            }
            return result;
        }

        private static string UploadImage(string imagePath)
        {
            var result = "";
            try
            {
                if (!File.Exists(imagePath)) return result;
                using (var w = new WebClient())
                {
                    w.Headers.Add("Authorization", "Client-ID " + DomainSettings.KeyImgurClientId);
                    var values = new NameValueCollection
                    {
                        {"image", MakeImageBase64(imagePath) }
                    };
                    var response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);
                    result = XDocument.Load(new MemoryStream(response)).Root?.Element("link")?.Value;
                }
            }
            catch (Exception e)
            {
                //ignored
            }
            return result;
        }

        private Attachment MakeAttachment(AttachmentType type, string path)
        {
            Attachment result = null;
            switch (type)
            {
                case AttachmentType.Local:
                    result = new Attachment(Current.Server.MapPath(path));
                    break;
                case AttachmentType.Web:
                    using (var sr = new WebClient{UseDefaultCredentials = true}.OpenRead(path))
                    {
                        if (sr != null) result = new Attachment(sr, "");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return result;
        }

        private MailMessage MakeMailMessage(MessageType messageType, string subject, string body,
            IEnumerable<string> to, Attachment[] attachment = null)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(DomainSettings.EmailInfo, MailParameters.DisplayNameInfo),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
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

        private Transmission MakeTransmission(MessageType messageType, string subject, string body,
            IEnumerable<string> to, SparkPost.Attachment[] attachment = null)
        {
            var transmission = new Transmission
            {
                Content =
                {
                    Subject = subject,
                    Html = body,
                    From =
                    {
                        Email = DomainSettings.EmailInfo,
                        Name = MailParameters.DisplayNameInfo
                    }
                }
            };
            switch (messageType)
            {
                case MessageType.Mass:
                    transmission.Recipients.Add(new Recipient {Address = {Email = DomainSettings.EmailNo_reply}});
                    foreach (var recipient in to)
                    {
                        transmission.Recipients.Add(new Recipient
                        {
                            Address =
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
                        transmission.Recipients.Add(new Recipient {Address = {Email = recipient}});
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
            if(attachment == null) return transmission;
            foreach (var a in attachment)
            {
                transmission.Content.Attachments.Add(a);
            }
            return transmission;
        }

        private void SendRegistrationDoneAdmin(UserModel model, UserViewModel viewModel = null)
        {
            try
            {
                var body = MakeBody(Paths.EmailRegistrationDoneUserAdmin, model.Name, model.LastName, model.Email, model.UserName, model.RegDate.Date);
                SendTransmision(MessageType.Personal, MailParameters.SubjectRegistrationDoneAdmin, body, new[]
                {
                    DomainSettings.EmailRegistrados
                });
            }
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendRegistrationDoneUser(UserModel model, UserViewModel viewModel = null)
        {
            try
            {
                var body = MakeBody(Paths.EmailRegistrationDoneUser);
                body = body.Replace("{username}", model.UserName);
                body = body.Replace("{contra}", model.Password);
                SendTransmision(MessageType.Personal, MailParameters.SubjectRegistrationDone, body, new[] {model.Email}/*,
                    new[]
                    {
                        MakeAttachment(AttachmentType.Web, MailParameters.VolanteTopogisWeb)
                    }*/);
            }
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendDeslinderRegistrationAdmin(DeslinderModel model, DeslindeViewModel viewModel = null)
        {
            if (model == null)
                if (viewModel != null)
                    model = new DeslinderModel
                    {
                        Nombre = viewModel.Nombre,
                        Apellido = viewModel.Apellido,
                        Telefono = viewModel.Telefono,
                        Movil = viewModel.Movil,
                        Correo = viewModel.Correo,
                        Ubicacion = viewModel.Ubicacion,
                        NoMatrical = viewModel.NoMatrical,
                        NoParsela = viewModel.NoParsela,
                        NoDistrito = viewModel.NoDistrito,
                        Municipio = viewModel.Municipio,
                        Provincia = viewModel.Provincia,
                        Area = viewModel.Area,
                        RegDate = viewModel.RegDate
                    };
                else return;
            try
            {
                var body = MakeBody(Paths.EmailDeslindeUserRegisteredAdmin, model.Nombre, model.Apellido, model.Telefono, model.Movil, model.Correo,
                    model.Ubicacion, model.NoMatrical, model.NoParsela, model.NoDistrito, model.Municipio,
                    model.Provincia, model.Area, model.RegDate.Date);
                SendTransmision(MessageType.Personal, MailParameters.SubjectDeslindeRegistrationAdmin, body,
                    new[] {DomainSettings.EmailDeslinde});
            }
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendDeslinderRegistrationUser(DeslinderModel model, DeslindeViewModel viewModel = null)
        {
            if (model == null)
                if (viewModel != null) model = new DeslinderModel {Correo = viewModel.Correo};
                else return;
            try
            {
                var body = MakeBody(Paths.EmailDeslindeUserRegisteredUser);
                body = body.Replace("{modelCorreo}", model.Correo);
                SendTransmision(MessageType.Personal, MailParameters.SubjectDeslindeRegistrationUser, body,
                    new[] {model.Correo});
            }
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendSubscribeDone(string email)
        {
            try
            {
                var body = MakeBody(Paths.EmailSubscribedDone);
                SendTransmision(MessageType.Personal, MailParameters.SubjectSubscribeDone, body, new[] { email }/*, new[]
                {
                    MakeAttachment(AttachmentType.Web, MailParameters.VolanteTopogisWeb)
                }*/);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private void SendNewDocumentMessage(DocumentModel model, IReadOnlyCollection<UserModel> to)
        {
            try
            {
                if (to.Count < 1) return;
                var body = MakeBody(Paths.EmailNewDocumentAdded);
                body = body.Replace("{title1}", model.Nombre);
                body = body.Replace("{categoria1}", model.SubCategoria);
                body = body.Replace("{descripcion1}", model.Descripcion);
                body = body.Replace("{urlDocument}", DomainSettings.UrlDocument + model.Id);
                body = body.Replace("{imageDocument}", UploadImage(Current.Server.MapPath("~" + model.ImagePath)));

                var emails = to.Select(informedUser => informedUser.Email).ToList();
                SendTransmision(MessageType.Personal, MailParameters.SubjectNewDocumentMessage, body, emails/*, new[]
                {
                    MakeAttachment(AttachmentType.Web, MailParameters.VolanteTopogisWeb)
                }*/);
            }
            catch (Exception e)
            {
                //Ignore
            }
        }

        private void SendHomeVideoUpload(HomeSliderVideo model, IReadOnlyCollection<UserModel> to,
            HomeSlideVideoViewModel viewModel = null)
        {
            if (model == null)
                if (viewModel != null) model = new HomeSliderVideo {UrlVideo = viewModel.UrlVideo};
                else return;
            try
            {
                if (to.Count < 1) return;
                var body = MakeBody(Paths.EmailHomeVideoAdded);
                body = body.Replace("{imgVideo}", Youtube.GetImageFromId(model.UrlVideo));
                var emails = to.Select(i => i.Email).ToList();
                SendTransmision(MessageType.Personal, MailParameters.SubjectHomeVideoUpload, body, emails);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private void SendContactUs(ContactUsModel model, ContactUsViewModel viewModel = null)
        {
            if (model == null)
                if (viewModel != null)
                    model = new ContactUsModel
                    {
                        Email = viewModel.Email,
                        Nombre = viewModel.Nombre,
                        Mensaje = viewModel.Mensaje
                    };
                else return;
            try
            {
                var body = MakeBody(Paths.EmailContactUsAdmin, model.Nombre, model.Email, model.Mensaje);
                SendTransmision(MessageType.Personal, MailParameters.SubjectContactUs, body,
                    new[] {DomainSettings.EmailContact});
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}