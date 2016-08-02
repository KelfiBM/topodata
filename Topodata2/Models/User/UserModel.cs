using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Topodata2.Models.User
{
    public class UserModel
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Informed { get; set; }
        public DateTime RegDate { get; set; }
        public string Rol { get; set; }
    }
}