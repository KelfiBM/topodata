using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Topodata2.Models;
using Topodata2.Models.Mail;
using Topodata2.Models.Service;

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
        public ActionResult Document( int id = 0)
        {
            if (Request.IsAuthenticated)
            {
                var document = ServiceManager.GetDocument(id);
                if (document != null)
                {
                    return View(document);
                }
                else
                {
                    return RedirectToAction("NotFound","Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "User",new {returnUrl = Request.Url});
            }
        }

        public ActionResult Documents( int? page, int idSub = 0, int idContenido = 0)
        {
            var documentList = ServiceManager.GetDocuments(idSub, idContenido);
            if (documentList != null && documentList.Count > 0)
            {
                var pageNumber = page ?? 1;
                var onePage = documentList.ToPagedList(pageNumber, 15);
                return View(onePage);
            }
            else
            {
                return RedirectToAction("NotFound","Error");
            }
        }

        public ActionResult Contenido(int id)
        {
            return View("Contenido",ServiceManager.GetSubCategorieById(id));
        }
    }
}