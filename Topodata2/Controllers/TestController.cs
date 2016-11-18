using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Topodata2.Managers;
using Topodata2.Models.Test;

namespace Topodata2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegistrationDoneUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrationDoneUser(SendRegistrationDone model)
        {
            TestManager.SendRegistrationDoneUser(DateTime.Parse(model.Fecha));
            return View();
        }
    }
}