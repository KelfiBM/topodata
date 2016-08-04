using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

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
    }
}