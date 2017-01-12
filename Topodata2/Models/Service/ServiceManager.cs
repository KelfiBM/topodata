using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.WebPages;

namespace Topodata2.Models.Service
{
    public static class ServiceManager
    {
        private static string Connection { get; } =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static bool DocumentsExist()
        {
            var result = false;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = "SELECT TOP 1 * FROM LastDocuments";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
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
                reader?.Dispose();
                com.Dispose();
                con.Close();
            }
            return result;
        }

        public static List<DocumentModel> GetLastDocumentsAdded(int id = 0, int count = 0)
        {
            List<DocumentModel> result = null;
            string query = $"SELECT TOP ";
            if (count == 0)
            {
                query = query + "(100) PERCENT ";
            }
            else
            {
                query = query + $"({count}) ";
            }
            query = query + "* FROM LastDocuments ";
            if (id != 0)
            {
                query = query + $"WHERE IdSubCategoria = {id} ";
            }
            query = query + "ORDER BY RegDate DESC";

                /*query = $"SELECT TOP ({count}) dbo.DetalleDocumento.Imagen, " + "dbo.Documento.Nombre, " +
                        "dbo.DetalleDocumento.FechaPublicacion, " + "dbo.Categoria.Descripcion AS Categoria, " +
                        "dbo.DetalleDocumento.Descripcion, " + "dbo.Documento.IdDocumento, " +
                        "dbo.Categoria.IdCategoria " + "FROM dbo.Categoria " + "INNER JOIN dbo.DetalleDocumento " +
                        "ON dbo.Categoria.IdCategoria = dbo.DetalleDocumento.IdCategoria " + "INNER JOIN dbo.Documento " +
                        "ON dbo.DetalleDocumento.idDocumento = dbo.Documento.IdDocumento " +
                        $"WHERE (dbo.Categoria.IdCategoria = {id}) " +
                        "ORDER BY dbo.DetalleDocumento.FechaPublicacion DESC";*/
            
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
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
                    result = new List<DocumentModel>();
                    while (reader.Read())
                    {
                        result.Add(new DocumentModel
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Descripcion = reader.GetString(2),
                            ImagePath = reader.GetString(3),
                            RegDate = reader.GetDateTime(4),
                            SubCategoria = reader.GetString(5),
                            IdSubCategorie = reader.GetInt32(6),
                            Contenido = reader.GetString(7),
                            IdContenido = reader.GetInt32(8),
                            Url = reader.GetString(9),
                            DescripcionHtml = !reader.IsDBNull(10) ? reader.GetString(10) : null
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
                com.CommandText = $"SELECT dbo.SubCategoria.IdSubCategoria, dbo.SubCategoria.Descripcion AS SubCategoria, dbo.SubCategoria.ImagePath FROM dbo.Categoria_SubCategoria INNER JOIN dbo.Categoria ON dbo.Categoria_SubCategoria.IdCategoria = dbo.Categoria.IdCategoria INNER JOIN dbo.SubCategoria ON dbo.Categoria_SubCategoria.IdSubCategoria = dbo.SubCategoria.IdSubCategoria WHERE (dbo.Categoria_SubCategoria.IdCategoria = @id)";
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
                            Descripcion = reader.GetString(1),
                            ImagePath = reader.GetString(2)
                        });
                    }
                }

            }
            catch (Exception e)
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
                com.CommandText = $"SELECT dbo.Contenido.IdContenido, " +
                                  $"dbo.Contenido.Descripcion, " +
                                  $"dbo.SubCategoria_Contenido.IdSubCategoria " +
                                  $"FROM dbo.SubCategoria_Contenido " +
                                  $"INNER JOIN dbo.SubCategoria " +
                                  $"ON dbo.SubCategoria_Contenido.IdSubCategoria = dbo.SubCategoria.IdSubCategoria " +
                                  $"INNER JOIN dbo.Contenido " +
                                  $"ON dbo.SubCategoria_Contenido.IdContenido = dbo.Contenido.IdContenido " +
                                  $"WHERE (dbo.SubCategoria_Contenido.IdSubCategoria = @id) " +
                                  $"ORDER BY dbo.Contenido.Descripcion";
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
                            Descripcion = reader.GetString(1),
                            IdSubCategorie = reader.GetInt32(2),
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
                            Descripcion = reader.GetString(1),
                            HtmlIcon = reader.GetString(2)
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

        public static List<DocumentModel> GetDocuments(int subCategorieId, int contenidoId)
        {
            List<DocumentModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT dbo.Documento.IdDocumento, " +
                                  $"dbo.Documento.Nombre, " +
                                  $"dbo.Documento.Descripcion, " +
                                  $"dbo.Documento.ImagePath, " +
                                  $"dbo.Documento.Url, " +
                                  $"dbo.SubCategoria.Descripcion AS SubCategoria, " +
                                  $"dbo.Contenido.Descripcion AS Contenido, " +
                                  $"dbo.Documento.RegDate, " +
                                  $"dbo.SubCategoria.ImagePath AS SubCategorieImage, " +
                                  $"dbo.Documento.IdSubCategoria " +
                                  $"FROM dbo.SubCategoria " +
                                  $"INNER JOIN dbo.Contenido " +
                                  $"INNER JOIN dbo.SubCategoria_Contenido " +
                                  $"ON dbo.Contenido.IdContenido = dbo.SubCategoria_Contenido.IdContenido " +
                                  $"ON dbo.SubCategoria.IdSubCategoria = dbo.SubCategoria_Contenido.IdSubCategoria " +
                                  $"INNER JOIN dbo.Documento " +
                                  $"ON dbo.SubCategoria_Contenido.IdSubCategoria = dbo.Documento.IdSubCategoria " +
                                  $"AND dbo.SubCategoria_Contenido.IdContenido = dbo.Documento.IdContenido " +
                                  $"WHERE (dbo.Documento.IdSubCategoria = @subid) " +
                                  $"AND (dbo.Documento.IdContenido = @contenidoid) " +
                                  $"ORDER BY dbo.Documento.RegDate DESC";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                com.Parameters.AddWithValue("subid", subCategorieId);
                com.Parameters.AddWithValue("contenidoid", contenidoId);
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<DocumentModel>();
                    while (reader.Read())
                    {
                        result.Add(new DocumentModel
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Descripcion = reader.GetString(2),
                            ImagePath = reader.GetString(3),
                            Url = reader.GetString(4),
                            SubCategoria = reader.GetString(5),
                            Contenido = reader.GetString(6),
                            RegDate = reader.GetDateTime(7),
                            SubCategorieImagePath = reader.GetString(8),
                            IdSubCategorie = reader.GetInt32(9)
                        });
                    }
                }

            }
            catch (Exception e)
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

        public static DocumentModel GetDocument(int id)
        {
            DocumentModel result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT dbo.Documento.IdDocumento, dbo.Documento.Nombre, dbo.Documento.Descripcion, dbo.Documento.ImagePath, dbo.Documento.Url, dbo.SubCategoria.Descripcion AS SubCategoria, dbo.Contenido.Descripcion AS Contenido, dbo.Documento.RegDate, dbo.SubCategoria.ImagePath AS SubCategorieImage, dbo.Documento.IdSubCategoria, dbo.Documento.DescripcionHtml FROM dbo.SubCategoria INNER JOIN dbo.Contenido INNER JOIN dbo.SubCategoria_Contenido ON dbo.Contenido.IdContenido = dbo.SubCategoria_Contenido.IdContenido ON dbo.SubCategoria.IdSubCategoria = dbo.SubCategoria_Contenido.IdSubCategoria INNER JOIN dbo.Documento ON dbo.SubCategoria_Contenido.IdSubCategoria = dbo.Documento.IdSubCategoria AND dbo.SubCategoria_Contenido.IdContenido = dbo.Documento.IdContenido WHERE (dbo.Documento.IdDocumento = @id)";
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("id", id);
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        result = new DocumentModel
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Descripcion = reader.GetString(2),
                            ImagePath = reader.GetString(3),
                            Url = reader.GetString(4),
                            SubCategoria = reader.GetString(5),
                            Contenido = reader.GetString(6),
                            RegDate = reader.GetDateTime(7),
                            SubCategorieImagePath = reader.GetString(8),
                            IdSubCategorie = reader.GetInt32(9),
                            DescripcionHtml = !reader.IsDBNull(10) ? reader.GetString(10) : null
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

        public static bool AddDocument(DocumentViewModel model)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            var query =
                "INSERT INTO [Topodata].[dbo].[Documento] (Nombre, Descripcion, ImagePath, Url, IdSubCategoria, IdContenido, DescripcionHtml) " +
                "VALUES (@nombre, @descripcion, @imagepath, @url, @idsubcategoria, @idcontenido, @descripcionHtml)";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                if (model.ImagePath == null || model.ImagePath.IsEmpty())
                {
                    model.ImagePath = "/resources/img/documents/logoDefault.png";
                }

                sqlCommand.Parameters.AddWithValue("nombre", model.Nombre);
                sqlCommand.Parameters.AddWithValue("descripcion", model.Descripcion);
                sqlCommand.Parameters.AddWithValue("imagepath", model.ImagePath);
                sqlCommand.Parameters.AddWithValue("url", model.Url);
                sqlCommand.Parameters.AddWithValue("idsubcategoria", model.IdSubCategoria);
                sqlCommand.Parameters.AddWithValue("idcontenido", model.IdContenido);
                sqlCommand.Parameters.AddWithValue("descripcionHtml", model.DescripcionHtml);
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

        public static DocumentModel GetLastDocument()
        {
            DocumentModel result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT TOP (1) dbo.Documento.IdDocumento, " +
                                  $"dbo.Documento.Nombre, " +
                                  $"dbo.Documento.Descripcion, " +
                                  $"dbo.Documento.ImagePath, " +
                                  $"dbo.Documento.Url, " +
                                  $"dbo.SubCategoria.Descripcion AS SubCategoria, " +
                                  $"dbo.Contenido.Descripcion AS Contenido, " +
                                  $"dbo.Documento.RegDate, " +
                                  $"dbo.SubCategoria.ImagePath AS SubCategorieImage, " +
                                  $"dbo.Documento.IdSubCategoria " +
                                  $"FROM dbo.SubCategoria " +
                                  $"INNER JOIN dbo.Contenido " +
                                  $"INNER JOIN dbo.SubCategoria_Contenido " +
                                  $"ON dbo.Contenido.IdContenido = dbo.SubCategoria_Contenido.IdContenido " +
                                  $"ON dbo.SubCategoria.IdSubCategoria = dbo.SubCategoria_Contenido.IdSubCategoria " +
                                  $"INNER JOIN dbo.Documento " +
                                  $"ON dbo.SubCategoria_Contenido.IdSubCategoria = dbo.Documento.IdSubCategoria " +
                                  $"AND dbo.SubCategoria_Contenido.IdContenido = dbo.Documento.IdContenido " +
                                  $"ORDER BY dbo.Documento.RegDate DESC ";
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();

                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        result = new DocumentModel
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Descripcion = reader.GetString(2),
                            ImagePath = reader.GetString(3),
                            Url = reader.GetString(4),
                            SubCategoria = reader.GetString(5),
                            Contenido = reader.GetString(6),
                            RegDate = reader.GetDateTime(7),
                            SubCategorieImagePath = reader.GetString(8),
                            IdSubCategorie = reader.GetInt32(9)
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

        public static List<SubCategorieModel> GetAllSubCategories()
        {
            List<SubCategorieModel> result = null;
            var con = new SqlConnection(Connection);
            var com = new SqlCommand();
            SqlDataReader reader = null;
            try
            {
                com.CommandText = $"SELECT dbo.SubCategoria.* FROM dbo.SubCategoria";
                com.CommandType = CommandType.Text;
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
                            Descripcion = reader.GetString(1),
                            ImagePath = reader.GetString(2),
                            RegDate = reader.GetDateTime(3)
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

        public static bool DeleteSubCategorie(int id)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            var query =
                "BEGIN;" +
                "DELETE FROM [Topodata].[dbo].[SubCategoria_Contenido] " +
                "WHERE IdSubCategoria = @id; " +
                "DELETE FROM [Topodata].[dbo].[SubCategoria] " +
                "WHERE IdSubCategoria = @id; " +
                "COMMIT;";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlCommand.Parameters.AddWithValue("id", id);
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

        public static bool EditCategorie(int id, string descripcion)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            string query = $"UPDATE [Topodata].[dbo].[SubCategoria] SET Descripcion = '{descripcion}' WHERE IdSubCategoria = {id}";
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

        public static bool DeleteDocument(int id)
        {
            var result = false;
            var sqlConnection = new SqlConnection(Connection);
            const string query = "BEGIN TRANSACTION " +
                                 "DELETE FROM [Topodata].[dbo].[Documento] " +
                                 "WHERE IdDocumento = @id " +
                                 "COMMIT";
            var sqlCommand = new SqlCommand(query, sqlConnection);
            try
            {
                sqlCommand.Parameters.AddWithValue("id", id);
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