using System;
using Topodata2.Classes;

namespace Topodata2.Models.User
{
    public class UserModel : Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Informed { get; set; }
        public DateTime RegDate { get; set; }
        public string Rol { get; set; }
    }
}