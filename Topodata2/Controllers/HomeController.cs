using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Topodata2.Models;

namespace Topodata2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<ServiceDocumentViewModel> serviceDocument =
                new ServiceDocumentViewModel().GetTopDocumentListByCategorie(0, 4);
            return View(serviceDocument);
        }

        [Route("SobreTopo")]
        public ActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Route("NuestroEquipo")]
        public ActionResult OurTeam()
        {

            return View();
        }

        [HttpGet]
        [Route("Contacto")]
        public ActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpGet]
        [Route("media/fotos")]
        public ActionResult Photo()
        {

            return View();
        }
        [HttpGet]
        [Route("media/videos")]
        public ActionResult Video()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string name, string mail, string message)
        {

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}