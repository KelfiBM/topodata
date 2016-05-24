using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
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
        [Route("Documento/{id:int}")]
        public ActionResult Document(int id = 0)
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

        public ActionResult Documents( int? page, int id = 0)
        {
            var serviceDocumentList = new ServiceDocumentViewModel().GetDocumentListByCategorie(id);
            if (serviceDocumentList != null && serviceDocumentList.Count > 0)
            {
                var pageNumber = page ?? 1;
                var onePage = serviceDocumentList.ToPagedList(pageNumber, 10);
                return View(onePage);
            }
            else
            {
                return HttpNotFound();
            }
            return null;
        }
    }
}