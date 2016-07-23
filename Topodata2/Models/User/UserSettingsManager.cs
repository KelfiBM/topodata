using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Topodata2.Models.User
{
    public static class UserSettingsManager
    {
        private static readonly string Connection =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static UserProfileSettingsNotificationViewModel GetCurrentUserProfileSettingsNotificationViewModel()
        {
            UserProfileSettingsNotificationViewModel userProfileSettingsNotificationViewModel = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT Informed FROM dbo.Users WHERE IdUsers = '{UserManager.User.Id}'";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        userProfileSettingsNotificationViewModel = new UserProfileSettingsNotificationViewModel
                        {
                            NewDocuments = reader.GetBoolean(0)
                        };
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
            return userProfileSettingsNotificationViewModel;
        }

        public static bool SaveUserSettingsNotification(UserProfileSettingsNotificationViewModel userProfileSettings)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            var query = "UPDATE [Topodata].[dbo].[Users] SET Informed = @informed WHERE IdUsers = @id";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlCommand.Parameters.AddWithValue("informed", userProfileSettings.NewDocuments);
                sqlCommand.Parameters.AddWithValue("id", Convert.ToInt32(UserManager.User.Id));
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