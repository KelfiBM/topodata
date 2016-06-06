using System.IO;
using System.Web;

namespace Topodata2.Models
{
    public class MessageTemplate
    {
        public string AddedNewContent(ServiceDocumentViewModel serviceDocument)
        {
            string addedNewContent;
            string title = serviceDocument.Nombre;
            string imagePath = serviceDocument.ImagePath;
            string categorie = serviceDocument.Categoria;
            string description = serviceDocument.Descripcion;
            using (
                StreamReader reader =
                    new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/NewContentAdded.html")))
            {
                addedNewContent = reader.ReadToEnd();
            }
            addedNewContent = addedNewContent.Replace("{0}",
                HttpContext.Current.Server.MapPath("~/resources/img/documents/logoDefault.png"));
            addedNewContent = addedNewContent.Replace("{1}", title);
            addedNewContent = addedNewContent.Replace("{2}", imagePath);
            addedNewContent = addedNewContent.Replace("{3}", categorie);
            addedNewContent = addedNewContent.Replace("{4}", description);
            return addedNewContent;
        }
    }
}