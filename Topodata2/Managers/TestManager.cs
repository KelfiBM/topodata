using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Topodata2.Models.Mail;
using Topodata2.Models.User;

namespace Topodata2.Managers
{
    public class TestManager
    {
        public static bool SendRegistrationDoneUser(DateTime fecha)
        {
            var result = false;
            try
            {
                foreach (var userModel in UserManager.GetAllUsers(fecha))
                {
                    new MailManager().SendMail(MailType.RegistrationDoneUser, userModel);
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