using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Topodata2.Controllers
{
    public class ServicesController : Controller
    {
        // GET: Services
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return View("Lockout");
            }
            else
            {
                return View("MustLogIn");
            }
        }
    }
}