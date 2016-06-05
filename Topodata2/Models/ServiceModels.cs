using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.WebPages;

namespace Topodata2.Models
{
    public class ServiceModels
    {

    }

    public class ServiceDocumentViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        public string Categoria { get; set; }

        public DateTime FechaPublicacion { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        public int IdCategoria { get; set; }

        public bool Exists { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }

        public string ImagePath { get; set; }

        public bool AddDocument(ServiceDocumentViewModel serviceDocument)
        {
            try
            {
                string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    string query =
                        "BEGIN TRANSACTION " +
                        "DECLARE @DocumentId int; " +
                        "INSERT INTO [dbo].[Documento] VALUES (@nombre); " +
                        "SELECT @DocumentID = scope_identity();" +
                        "INSERT INTO [dbo].[DetalleDocumento] VALUES (@DocumentID, @descripcion, @idCategoria, @fechaPublicacion, @imagePath, @url);" +
                        "COMMIT";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    if (serviceDocument.ImagePath == null || serviceDocument.ImagePath.IsEmpty())
                    {
                        serviceDocument.ImagePath = "/resources/img/documents/logoDefault.png";
                    }
                    sqlCommand.Parameters.AddWithValue("nombre", serviceDocument.Nombre);
                    sqlCommand.Parameters.AddWithValue("descripcion", serviceDocument.Descripcion);
                    sqlCommand.Parameters.AddWithValue("idCategoria", serviceDocument.IdCategoria);
                    sqlCommand.Parameters.AddWithValue("fechaPublicacion", DateTime.Now);
                    sqlCommand.Parameters.AddWithValue("imagePath", serviceDocument.ImagePath);
                    sqlCommand.Parameters.AddWithValue("url", serviceDocument.Url);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                return true;
            }
            catch (SqlException es)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

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
                        serviceDocument.IdCategoria = reader.GetInt32(5);
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

        public ServiceDocumentViewModel GetLastDocummentAdded()
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
                        "SELECT TOP 1 dbo.Documento.Nombre, " +
                        "dbo.DetalleDocumento.Descripcion, " +
                        "dbo.Categoria.Descripcion AS Categoria, " +
                        "dbo.DetalleDocumento.FechaPublicacion, " +
                        "dbo.DetalleDocumento.Url, " +
                        "dbo.Categoria.IdCategoria, " +
                        "dbo.Documento.IdDocumento " +
                        "FROM dbo.Categoria INNER JOIN dbo.DetalleDocumento " +
                        "ON dbo.Categoria.IdCategoria = dbo.DetalleDocumento.IdCategoria " +
                        "INNER JOIN dbo.Documento " +
                        "ON dbo.DetalleDocumento.idDocumento = dbo.Documento.IdDocumento " +
                        "ORDER BY dbo.DetalleDocumento.FechaPublicacion DESC");
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
                        serviceDocument.IdCategoria = reader.GetInt32(5);
                        serviceDocument.Id = reader.GetInt32(6);
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

        public List<ServiceDocumentViewModel> GetDocumentListByCategorie(int id)
        {
            string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string query;
            if (id == 0)
            {
                query = "SELECT dbo.DetalleDocumento.Imagen, " +
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
                        "ORDER BY dbo.DetalleDocumento.FechaPublicacion DESC";
            }
            else
            {
                query = string.Format(
                        "SELECT dbo.DetalleDocumento.Imagen, " +
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
                        "WHERE (dbo.Categoria.IdCategoria = {0}) " +
                        "ORDER BY dbo.DetalleDocumento.FechaPublicacion DESC", id);
            }
            List<ServiceDocumentViewModel> serviceDocumentList = new List<ServiceDocumentViewModel>();
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand com = new SqlCommand();
                SqlDataReader reader;
                com.CommandText = query;
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();
                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var serviceDocument = new ServiceDocumentViewModel();

                        serviceDocument.ImagePath = reader.GetString(0);
                        serviceDocument.Nombre = reader.GetString(1);
                        serviceDocument.FechaPublicacion = reader.GetDateTime(2);
                        serviceDocument.Categoria = reader.GetString(3);
                        serviceDocument.Descripcion = reader.GetString(4);
                        serviceDocument.Id = reader.GetInt32(5);
                        serviceDocument.IdCategoria = reader.GetInt32(6);
                        serviceDocument.Exists = true;
                        serviceDocumentList.Add(serviceDocument);
                    }
                    return serviceDocumentList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ServiceDocumentViewModel> GetTopDocumentListByCategorie(int id, int count)
        {
            string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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
                        "ORDER BY dbo.DetalleDocumento.FechaPublicacion DESC",count);
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
                        "ORDER BY dbo.DetalleDocumento.FechaPublicacion DESC",count, id);
            }
            List<ServiceDocumentViewModel> serviceDocumentList = new List<ServiceDocumentViewModel>();
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand com = new SqlCommand();
                SqlDataReader reader;
                com.CommandText = query;
                com.CommandType = CommandType.Text;
                com.Connection = con;
                con.Open();
                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var serviceDocument = new ServiceDocumentViewModel();

                        serviceDocument.ImagePath = reader.GetString(0);
                        serviceDocument.Nombre = reader.GetString(1);
                        serviceDocument.FechaPublicacion = reader.GetDateTime(2);
                        serviceDocument.Categoria = reader.GetString(3);
                        serviceDocument.Descripcion = reader.GetString(4);
                        serviceDocument.Id = reader.GetInt32(5);
                        serviceDocument.IdCategoria = reader.GetInt32(6);
                        serviceDocument.Exists = true;
                        serviceDocumentList.Add(serviceDocument);
                    }
                    return serviceDocumentList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}