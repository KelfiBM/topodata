using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Topodata2.Models.Home
{
    public static class HomeManager
    {
        private static readonly string Connection =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static HomeTextPrincipalViewModel GetLastHomeText()
        {
            HomeTextPrincipalViewModel result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT TOP (100) PERCENT dbo.TextoHome.* FROM dbo.TextoHome ORDER BY regDate DESC";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        result = new HomeTextPrincipalViewModel()
                        {
                            IdHomeText = reader.GetInt32(0),
                            Agrimensura = reader.GetString(1),
                            EstudioSuelo = reader.GetString(2),
                            Diseno = reader.GetString(3),
                            Ingenieria = reader.GetString(4),
                            RegDate = reader.GetDateTime(5).Date
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
            return result;
        }

        public static HomeSliderViewModel GetCurrentHomeSlider()
        {
            HomeSliderViewModel result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT TOP (1) IdVideoHome, UrlVideo, regDate FROM dbo.HomeSlide ORDER BY regDate DESC";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        result = new HomeSliderViewModel()
                        {
                            IdVideoHome = reader.GetInt32(0),
                            UrlVideo = reader.GetString(1),
                            RegDate = reader.GetDateTime(2)
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
            return result;
        }

        public static bool AddHomeText(HomeTextPrincipalViewModel viewmodel)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            var query =
                "INSERT INTO [Topodata].[dbo].[TextoHome] (Agrimensura, EstudioSuelo, Diseno, Ingenieria) VALUES (@agrimensura, @estudiosuelo, @diseno, @ingenieria)";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlCommand.Parameters.AddWithValue("agrimensura", viewmodel.Agrimensura);
                sqlCommand.Parameters.AddWithValue("estudiosuelo", viewmodel.EstudioSuelo);
                sqlCommand.Parameters.AddWithValue("diseno", viewmodel.Diseno);
                sqlCommand.Parameters.AddWithValue("ingenieria", viewmodel.Ingenieria);
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

        public static bool AddHomeSlideVideo(HomeSliderViewModel viewmodel)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            var query =
                "INSERT INTO [Topodata].[dbo].[HomeSlide] (UrlVideo) VALUES (@urlVideo)";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlCommand.Parameters.AddWithValue("urlVideo", viewmodel.UrlVideo);
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
