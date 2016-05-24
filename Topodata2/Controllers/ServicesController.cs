using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Topodata2.Models;

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

        public ActionResult Document(int id)
        {
            var serviceDocument = new ServiceDocumentViewModel().GetDocumentById(id);
            if (serviceDocument.Exists)
            {
                return View(serviceDocument);
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}