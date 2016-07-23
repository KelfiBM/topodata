using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Topodata2.Models.User
{
    public static class UserManager
    {
        public static User User
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return ((MyPrincipal) (HttpContext.Current.User)).User;
                }
                if (HttpContext.Current.Items.Contains("User"))
                {
                    return (User) HttpContext.Current.Items["User"];
                }
                return null;
            }
        }

        public static User AuthenticateUser(string username, string password)
        {
            User user = null;
            using (
                var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
                )
            {
                string query =
                    "SELECT [IdUsers],[Name],[Lastname],[Email],[Username] FROM [Users] WHERE [Username] = @u AND [Password] = @p";
                var com = new SqlCommand(query, con);
                com.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = username;
                com.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar))
                    .Value = password;
                con.Open();
                var reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Username = reader.GetString(4)
                        };
                    }
                }
                reader.Dispose();
                com.Dispose();
                con.Dispose();
                return user;
            }
        }

        public static bool ValidateUser(LoginUserViewModel loginUserViewModel, HttpResponseBase response)
        {
            bool result = false;
            if (Membership.ValidateUser(loginUserViewModel.Username, loginUserViewModel.Password))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string userData = serializer.Serialize(UserManager.User);

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2,
                    loginUserViewModel.Username,
                    DateTime.Now,
                    DateTime.Now.AddYears(10),
                    loginUserViewModel.KeepConnected,
                    userData,
                    FormsAuthentication.FormsCookiePath);
                string ecryptTicket = FormsAuthentication.Encrypt(ticket);

                response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, ecryptTicket));
                result = true;
            }
            return result;
        }

        public static void Logout(HttpSessionStateBase session, HttpResponseBase response)
        {
            session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            response.Cookies.Add(cookie);
        }

        public static bool IsUserInRole(string role)
        {
            var result = new CustomRoleProvider().IsUserInRole(User.Username, role);
            return result;
        }
    }    
}