using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Topodata2.Models.Service
{
    public static class ServiceManager
    {
        private static string Connection { get; } =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static List<ServiceDocumentViewModel> GetLastDocumentsAdded(int id, int count)
        {
            List<ServiceDocumentViewModel> result = null;
            string query;
            if (id == 0)
            {
                query = string.Format("SELECT TOP ({0}) dbo.DetalleDocumento.Imagen, " +
                        "dbo.Documento.Nombre, " +
                        "dbo.DetalleDocumento.FechaPublicacion, " +
                        "dbo.Categoria.Descripcion AS Categoria, " +
                        "dbo.DetalleDocumento.Descripcion, " +
                        "dbo.Documento.IdDocumento, " +
                        "dbo.Categoria.IdCategoria " +
                        "FROM dbo.Categoria " +
                        "INNER JOIN dbo.DetalleDocumento " +
                        "ON dbo.Categoria.IdCategoria = dbo.DetalleDocumento.IdCategoria " +
                        "INNER JOIN dbo.Documento " +
                        "ON dbo.DetalleDocumento.idDocumento = dbo.Documento.IdDocumento " +
                        "ORDER BY dbo.DetalleDocumento.FechaPublicacion DESC", count);
            }
            else
            {
                query = string.Format(
                        "SELECT TOP ({0}) dbo.DetalleDocumento.Imagen, " +
                        "dbo.Documento.Nombre, " +
                        "dbo.DetalleDocumento.FechaPublicacion, " +
                        "dbo.Categoria.Descripcion AS Categoria, " +
                        "dbo.DetalleDocumento.Descripcion, " +
                        "dbo.Documento.IdDocumento, " +
                        "dbo.Categoria.IdCategoria " +
                        "FROM dbo.Categoria " +
                        "INNER JOIN dbo.DetalleDocumento " +
                        "ON dbo.Categoria.IdCategoria = dbo.DetalleDocumento.IdCategoria " +
                        "INNER JOIN dbo.Documento " +
                        "ON dbo.DetalleDocumento.idDocumento = dbo.Documento.IdDocumento " +
                        "WHERE (dbo.Categoria.IdCategoria = {1}) " +
                        "ORDER BY dbo.DetalleDocumento.FechaPublicacion DESC", count, id);
            }
            var con = new SqlConnection(Connection);
            SqlCommand com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {

                com.CommandText = query;
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();
                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<ServiceDocumentViewModel>();
                    while (reader.Read())
                    {
                        result.Add(new ServiceDocumentViewModel()
                        {
                            ImagePath = reader.GetString(0),
                            Nombre = reader.GetString(1),
                            FechaPublicacion = reader.GetDateTime(2),
                            Categoria = reader.GetString(3),
                            Descripcion = reader.GetString(4),
                            Id = reader.GetInt32(5),
                            IdCategoria = reader.GetInt32(6),
                            Exists = true
                        });
                    }
                }
            }
                // ReSharper disable once UnusedVariable
            catch (Exception exception)
            {
                //Ignore
            }
            finally
            {
                reader?.Dispose();
                com.Dispose();
                con.Close();
            }
            return result;
        }

        public static List<SubCategorieModel> GetSubCategoriesByCategorieId(CategorieModel model)
        {
            List<SubCategorieModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT dbo.SubCategoria.IdSubCategoria, dbo.SubCategoria.Descripcion AS SubCategoria FROM dbo.Categoria_SubCategoria INNER JOIN dbo.Categoria ON dbo.Categoria_SubCategoria.IdCategoria = dbo.Categoria.IdCategoria INNER JOIN dbo.SubCategoria ON dbo.Categoria_SubCategoria.IdSubCategoria = dbo.SubCategoria.IdSubCategoria WHERE (dbo.Categoria_SubCategoria.IdCategoria = @id)";
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("id", model.Id);
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<SubCategorieModel>();
                    while (reader.Read())
                    {
                        result.Add(new SubCategorieModel
                        {
                            Id = reader.GetInt32(0),
                            Descripcion = reader.GetString(1)
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

        public static List<ContenidoModel> GetAllContenidoBySubcategorieId(SubCategorieModel model)
        {
            List<ContenidoModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT dbo.Contenido.IdContenido, dbo.Contenido.Descripcion FROM dbo.SubCategoria_Contenido INNER JOIN dbo.SubCategoria ON dbo.SubCategoria_Contenido.IdSubCategoria = dbo.SubCategoria.IdSubCategoria INNER JOIN dbo.Contenido ON dbo.SubCategoria_Contenido.IdContenido = dbo.Contenido.IdContenido WHERE (dbo.SubCategoria_Contenido.IdSubCategoria = @id) ORDER BY dbo.Contenido.Descripcion";
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("id", model.Id);
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<ContenidoModel>();
                    while (reader.Read())
                    {
                        result.Add(new ContenidoModel
                        {
                            Id = reader.GetInt32(0),
                            Descripcion = reader.GetString(1)
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

        public static List<CategorieModel> GetAllCategories()
        {
            List<CategorieModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT dbo.Categoria.* FROM dbo.Categoria";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<CategorieModel>();
                    while (reader.Read())
                    {
                        result.Add(new CategorieModel
                        {
                            Id = reader.GetInt32(0),
                            Descripcion = reader.GetString(1)
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

        public static SubCategorieModel GetSubCategorieById(int id)
        {
            SubCategorieModel result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT dbo.SubCategoria.* FROM dbo.SubCategoria WHERE (IdSubCategoria = @id)";
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("id", id);
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        result = new SubCategorieModel
                        {
                            Id = reader.GetInt32(0),
                            Descripcion = reader.GetString(1)
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
    }
}