using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Topodata2.Models
{
    public sealed class CustomRoleProvider : RoleProvider
    {
        private string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand com = new SqlCommand();
                SqlDataReader reader;
                com.CommandText = string.Format("SELECT dbo.Roles.Descripcion, dbo.Users.Username FROM dbo.Roles INNER JOIN dbo.Users ON dbo.Roles.idRole = dbo.Users.idRole WHERE(dbo.Users.Username = N'{0}') AND(dbo.Roles.Descripcion = N'{1}')", username,roleName);
                com.CommandType = CommandType.Text;
                com.Connection = con;

                con.Open();
                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    com.Dispose();
                    con.Close();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    com.Dispose();
                    con.Close();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand com = new SqlCommand();
                SqlDataReader reader;
                com.CommandText =
                    string.Format(
                        "SELECT dbo.Roles.Descripcion FROM dbo.Roles INNER JOIN dbo.Users ON dbo.Roles.idRole = dbo.Users.idRole WHERE (dbo.Users.Username = N'{0}')",
                        username);
                com.CommandType = CommandType.Text;
                com.Connection = con;

                con.Open();
                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    List<string> roles = new List<string>();
                    if (reader.Read())
                    {
                       roles.Add(reader.GetString(0));
                    }
                    reader.Dispose();
                    com.Dispose();
                    con.Close();
                    return roles.ToArray();
                }

                reader.Dispose();
                com.Dispose();
                con.Close();
                return new string[] {};

            }
            catch (Exception)
            {
                return new string[] { }; ;
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}