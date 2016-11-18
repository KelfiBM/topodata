using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Topodata2.Models.Mail;

namespace Topodata2.Models.User
{
    public static class UserManager
    {
        private static readonly string Connection =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

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
                const string query = "SELECT [IdUsers],[Name],[Lastname],[Email],[Username] FROM [Users] WHERE ([Username] = @u AND [Password] = @p) OR ([Email] = @u AND [Password] = @p)";
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

        public static bool IsActualPassword(string actualPassword)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            SqlDataReader reader;
            string query = $"SELECT [Password] FROM [Topodata].[dbo].[Users] WHERE (Password = '{actualPassword}') AND (IdUsers = {User.Id})";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                // ignored
            }
            finally
            {
                sqlCommand.Dispose();
                sqlConnection.Dispose();
            }
            return result;
        }

        public static bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var result = false;
            if (!IsActualPassword(oldPassword))
            {
                return false;
            }
            var sqlConnection = new SqlConnection(Connection);
            string query = $"UPDATE [Topodata].[dbo].[Users] SET Password = '{newPassword}' WHERE IdUsers = {User.Id}";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                result = true;
            }
            catch (Exception e)
            {
                // ignored
            }
            finally
            {
                sqlCommand.Dispose();
                sqlConnection.Dispose();
            }
            return result;
        }

        public static bool ExistsInformedUsers()
        {
            bool result = false;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT 1 FROM dbo.Users WHERE (dbo.Users.Informed = 1)";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                reader?.Dispose();
                com.Dispose();
                con.Close();
            }
            return result;
        }

        public static bool ExistsSuscribedInformed()
        {
            bool result = false;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT 1 FROM viewSubscribedInformed";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                reader?.Dispose();
                com.Dispose();
                con.Close();
            }
            return result;
        }

        public static List<UserModel> GetSuscribedInformed()
        {
            List<UserModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT Email FROM viewSubscribedInformed";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<UserModel>();
                    while (reader.Read())
                    {
                        result.Add(new UserModel()
                        {
                            Email = reader.GetString(0),
                        });
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                reader?.Dispose();
                com.Dispose();
                con.Close();
            }
            return result;
        }

        public static List<UserModel> GetAllInformedUsers()
        {
            List<UserModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT * FROM viewAllUsers WHERE viewAllUsers.Informed = 1";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<UserModel>();
                    while (reader.Read())
                    {
                        result.Add(new UserModel()
                        {
                            Name = reader.GetString(0),
                            LastName = reader.GetString(1),
                            Email = reader.GetString(2),
                            UserName = reader.GetString(3),
                            Informed = reader.GetBoolean(4),
                            RegDate = reader.GetDateTime(5),
                            Rol = reader.GetString(6),
                            IdUser = reader.GetInt32(7),
                            Password = reader.GetString(8)
                        });
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                reader?.Dispose();
                com.Dispose();
                con.Close();
            }
            return result;
        }
        
        public static List<UserModel> GetAllUsers()
        {
            List<UserModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT * FROM viewAllUsers";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<UserModel>();
                    while (reader.Read())
                    {
                        result.Add(new UserModel()
                        {
                            Name = reader.GetString(0),
                            LastName = reader.GetString(1),
                            Email = reader.GetString(2),
                            UserName = reader.GetString(3),
                            Informed = reader.GetBoolean(4),
                            RegDate = reader.GetDateTime(5),
                            Rol = reader.GetString(6),
                            IdUser = reader.GetInt32(7),
                            Password = reader.GetString(8)
                        });
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                reader?.Dispose();
                com.Dispose();
                con.Close();
            }
            return result;
        }

        public static List<UserModel> GetAllUsers(DateTime fromDate)
        {
            List<UserModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT * FROM viewAllUsers " +
                                  $"WHERE viewAllUsers.RegDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'";

                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<UserModel>();
                    while (reader.Read())
                    {
                        result.Add(new UserModel()
                        {
                            Name = reader.GetString(0),
                            LastName = reader.GetString(1),
                            Email = reader.GetString(2),
                            UserName = reader.GetString(3),
                            Informed = reader.GetBoolean(4),
                            RegDate = reader.GetDateTime(5),
                            Rol = reader.GetString(6),
                            IdUser = reader.GetInt32(7),
                            Password = reader.GetString(8)
                        });
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                reader?.Dispose();
                com.Dispose();
                con.Close();
            }
            return result;
        } 

        public static bool DeleteUser(UserModel model)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            var query =
                "DELETE FROM [Topodata].[dbo].[Users] " +
                "WHERE IdUsers = @id";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlCommand.Parameters.AddWithValue("id", model.IdUser);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                result = true;
            }
            catch (Exception e)
            {
                // ignored
            }
            finally
            {
                sqlCommand.Dispose();
                sqlConnection.Dispose();
            }
            return result;
        }
    }    
}