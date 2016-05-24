using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Topodata2.Models
{
    public class ServiceModels
    {

    }

    public class ServiceDocumentViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string Url { get; set; }
        public int idCategoria { get; set; }
        public bool Exists { get; set; }

        public ServiceDocumentViewModel GetDocumentById(int id)
        {

            string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            ServiceDocumentViewModel serviceDocument = new ServiceDocumentViewModel();
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand com = new SqlCommand();
                SqlDataReader reader;
                com.CommandText =
                    string.Format(
                        "SELECT dbo.Documento.Nombre, " +
                        "dbo.DetalleDocumento.Descripcion, " +
                        "dbo.Categoria.Descripcion AS Categoria, " +
                        "dbo.DetalleDocumento.FechaPublicacion, " +
                        "dbo.DetalleDocumento.Url, " +
                        "dbo.Categoria.IdCategoria " +
                        "FROM dbo.Categoria INNER JOIN dbo.DetalleDocumento " +
                        "ON dbo.Categoria.IdCategoria = dbo.DetalleDocumento.IdCategoria " +
                        "INNER JOIN dbo.Documento " +
                        "ON dbo.DetalleDocumento.idDocumento = dbo.Documento.IdDocumento " +
                        "WHERE(dbo.Documento.IdDocumento = '{0}')",
                        id);
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();
                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        serviceDocument.Nombre = reader.GetString(0);
                        serviceDocument.Descripcion = reader.GetString(1);
                        serviceDocument.Categoria = reader.GetString(2);
                        serviceDocument.FechaPublicacion = reader.GetDateTime(3);
                        serviceDocument.Url = reader.GetString(4);
                        serviceDocument.idCategoria = reader.GetInt32(5);
                        serviceDocument.Exists = true;
                        return serviceDocument;
                    }
                    else
                    {
                        serviceDocument.Exists = false;
                        return serviceDocument;
                    }
                }
                else
                {
                    serviceDocument.Exists = false;
                    return serviceDocument;
                }
            }
            catch (Exception)
            {
                serviceDocument.Exists = false;
                return serviceDocument;
            }
        }
    }
}