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

        public static bool AddOurTeam(OurTeamViewModel viewmodel)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            var query =
                "INSERT INTO [Topodata].[dbo].[OurTeam] (Nombre, Cargo, Email, Imagen) " +
                "VALUES (@nombre, @cargo, @email, @imagen)";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlCommand.Parameters.AddWithValue("nombre", viewmodel.Nombre);
                sqlCommand.Parameters.AddWithValue("cargo", viewmodel.Cargo);
                sqlCommand.Parameters.AddWithValue("email", viewmodel.Email);
                sqlCommand.Parameters.AddWithValue("imagen", viewmodel.ImagePath);
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

        public static List<OurTeamViewModel> GetAllOurTeam()
        {
            List<OurTeamViewModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT * FROM dbo.OurTeam";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<OurTeamViewModel>();
                    while (reader.Read())
                    {
                        result.Add(new OurTeamViewModel()
                        {
                            IdOurTeam = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Cargo = reader.GetString(2),
                            Email = reader.GetString(3),
                            ImagePath = reader.GetString(4)
                        });
                    }
                }
                else
                {
                    AddOurTeam(new OurTeamViewModel()
                    {
                        Nombre = "Ignorar",
                        Cargo = "Ignorar",
                        Email = "Ignorar",
                        ImagePath = "Ignorar"
                    });
                    result = GetAllOurTeam();
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

        public static bool DeleteOurTeam(OurTeamViewModel viewmodel)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            var query =
                "DELETE FROM [Topodata].[dbo].[OurTeam] " +
                "WHERE IdOurTeam = @id";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                if (System.IO.File.Exists(GetImagePathOurTeam(viewmodel)))
                {
                    System.IO.File.Delete(GetImagePathOurTeam(viewmodel));
                }
                sqlCommand.Parameters.AddWithValue("id", viewmodel.IdOurTeam);
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

        private static string GetImagePathOurTeam(OurTeamViewModel viewmodel)
        {
            string result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT Imagen FROM dbo.OurTeam WHERE (IdOurTeam = @id)";
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("id", viewmodel.IdOurTeam);
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        result = reader.GetString(0);
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

    }
}
