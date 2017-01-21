using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Topodata2.Classes;
using Topodata2.Managers;
using Topodata2.resources.Strings;

namespace Topodata2.Models.User
{
    public static class UserManager
    {
        public static UserModel User
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return ((MyPrincipal) (HttpContext.Current.User)).User;
                }
                if (HttpContext.Current.Items.Contains("User"))
                {
                    return (UserModel) HttpContext.Current.Items["User"];
                }
                return null;
            }
        }

        public static bool ValidateUser(LoginUserViewModel loginUserViewModel, HttpResponseBase response)
        {
            if (!Membership.ValidateUser(loginUserViewModel.Username, loginUserViewModel.Password)) return false;
            var serializer = new JavaScriptSerializer();
            var userData = serializer.Serialize(User);

            var ticket = new FormsAuthenticationTicket(2,
                loginUserViewModel.Username,
                DateTime.Now,
                DateTime.Now.AddYears(10),
                loginUserViewModel.KeepConnected,
                userData,
                FormsAuthentication.FormsCookiePath);
            var ecryptTicket = FormsAuthentication.Encrypt(ticket);

            response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, ecryptTicket));
            return true;
        }

        public static void Logout(HttpSessionStateBase session, HttpResponseBase response)
        {
            session.Abandon();
            FormsAuthentication.SignOut();
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "") {Expires = DateTime.Now.AddYears(-1)};
            response.Cookies.Add(cookie);
        }

        public static List<UserModel> GetAllInformed()
        {
            var informed = new List<UserModel>();
            var informedUsers = GetInformedUsers();
            if (informedUsers != null) informed.AddRange(informedUsers);
            var informedSubscribed = GetSuscribed();
            if (informedSubscribed != null) informed.AddRange(informedSubscribed);
            return informed.Count == 0 ? null : informed;
        }

        public static List<List<UserModel>> GetAllInformedSeparated(int count)
        {
            var informed = GetAllInformed();
            if (informed == null) return null;
            var list = new List<UserModel>();
            var result = new List<List<UserModel>>();
            foreach (var i in informed)
            {
                list.Add(i);
                if (list.Count != count) continue;
                result.Add(list);
                list.Clear();
            }
            result.Add(list);
            return result;
        }

        //-----------------------------------------------------------------//

        public static bool DeleteUser(int id)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.DeleteUser, id.ToString());
            if (value.Count > 0) result = true;
            return result;
        }

        public static bool UpdateSubscribed(string mail, bool informed)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.UpdateInformed, mail, informed.ToString());
            if (value.Count > 0) result = true;
            return result;
        }

        public static UserModel GetUser(int id)
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.User,
                DatabaseParameters.GetUserFromId, id.ToString());
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (UserModel) i)[0];
            return result;
        }

        public static UserModel GetUser(string email)
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.User,
                DatabaseParameters.GetUserFromEmail, email);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (UserModel) i)[0];
            return result;
        }

        public static List<UserModel> GetAllUsers(DateTime fromDate)
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.User,
                string.Format(DatabaseParameters.GetAllUserFromDate, fromDate.ToString("yyyy-MM-dd")));
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (UserModel) i);
            return result;
        }

        public static List<UserModel> GetAllUsers()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.User,
                DatabaseParameters.GetAllUser);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (UserModel) i);
            return result;
        }

        public static List<UserModel> GetInformedUsers()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.User,
                DatabaseParameters.GetAllInformedUsers);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (UserModel) i);
            return result;
        }

        public static List<UserModel> GetSuscribed()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.Subscribed,
                DatabaseParameters.GetAllSubscribed);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (UserModel) i);
            return result;
        }

        public static UserModel AuthenticateUser(string username, string password)
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.User,
                DatabaseParameters.AuthenticateUser, username, password);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (UserModel) i)[0];
            return result;
        }

        public static bool IsActualPassword(string actualPassword)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.IsActualPassword, actualPassword, User.Id.ToString());
            if (value.Count > 0) result = true;
            return result;
        }

        public static bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!IsActualPassword(oldPassword)) return false;
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.ChangePassword, newPassword, User.Id.ToString());
            if (value.Count > 0) result = true;
            return result;
        }

        public static bool IsUserInRole(string roleName)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.UserIsInRole, User.Username, roleName);
            if (value.Count > 0) result = true;
            return result;
        }
    }
}