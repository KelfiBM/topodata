using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Topodata2.Models
{
    public class AllDocumentsModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string SubCategoria { get; set; }
        public string Contenido { get; set; }
        public string RegDate { get; set; }
        public string Url { get; set; }

    }
    public class AllFlipboardModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string RegDate { get; set; }

    }
    public class AllUsersModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Informed { get; set; }
        public string Rol { get; set; }
        public string RegDate { get; set; }

    }
}