using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Topodata2.Models.User
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class MyPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public User User { get; set; } 

        public MyPrincipal(IIdentity identity)
        {
            Identity = identity;
        }

        public bool IsInRole(string role)
        {
            return UserManager.IsUserInRole(role);
        }

    }
}