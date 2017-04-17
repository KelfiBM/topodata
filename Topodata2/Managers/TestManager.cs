using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Topodata2.Models.Mail;
using Topodata2.Models.UserFolder;

namespace Topodata2.Managers
{
    public class TestManager
    {
        public static void SendRDoneUser(DateTime fecha)
        {
            var thread = new Thread(() => SendRegistrationDoneUser(fecha));
            thread.Start();
        }

        public static bool SendRegistrationDoneUser(DateTime fecha)
        {

            var result = false;
            try
            {
                foreach (var userModel in UserManager.GetAllUsers(fecha))
                {
                    MailManager.SendMail(MailType.RegistrationDoneUser, userModel);
                }
                result = true;
            }
            catch (Exception e)
            {
                // ignored
            }

            return result;
        }
    }
}