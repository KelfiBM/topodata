using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace Topodata.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("SobreTopo")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Route("NuestroEquipo")]
        public IActionResult OurTeam()
        {

            return View();
        }

        [HttpGet]
        [Route("Contacto")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpGet]
        [Route("media/fotos")]
        public IActionResult Photo()
        {

            return View();
        }
        [HttpGet]
        [Route("media/videos")]
        public IActionResult Video()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Contact(string name, string mail, string message)
        {

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
