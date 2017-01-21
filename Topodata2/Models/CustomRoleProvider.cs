using System;
using System.Web.Security;

namespace Topodata2.Models
{
    public sealed class CustomRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            throw new NotImplementedException();
            /*try
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
            }*/
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