using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Topodata2.Models;
using Topodata2.Models.Home;
using Topodata2.Models.Mail;
using Topodata2.resources.Strings;

namespace Topodata2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var serviceDocument =
                new ServiceDocumentViewModel().GetTopDocumentListByCategorie(0, 4);
            return View("Index/Index",serviceDocument);
        }

        [Route("SobreTopo")]
        public ActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View("About/About");
        }

        [Route("NuestroEquipo")]
        public ActionResult OurTeam()
        {
            var model = HomeManager.GetAllOurTeam();
            return View(model ?? new List<OurTeam>());
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

        public ActionResult Contact()
        {
            return View("Contact");
        }

        [HttpPost]
        public ActionResult Contact(ContactUsViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("Contact", "Home");
            }
            try
            {
                new MailManager().SendMail(MailType.ContactUs, viewModel);
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("Contact", "Home");
            }
            catch (Exception)
            {
                // ignored
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("Contact", "Home");
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Deslinder()
        {
            return View("Deslinder");
        }
        [HttpPost]
        public ActionResult Deslinder(DeslindeViewModel viewModel)
        {
            string message;
            viewModel.RegDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                message = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = message;
                return RedirectToAction("Deslinder", "Home");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            try
            {
                new MailManager().SendMail(MailType.DeslinderRegistrationAdmin, viewModel)
                    .SendMail(MailType.DeslinderRegistrationUser, viewModel);
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("Deslinder", "Home");
            }
            catch
            {
                // ignored
            }
            message = Messages.ErrorDesconocido;
            TempData["OperationMessage"] = message;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = message;
            return RedirectToAction("Deslinder", "Home");
        }
    }
}