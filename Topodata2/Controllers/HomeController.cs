using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Topodata2.Classes;
using Topodata2.Managers;
using Topodata2.Models;
using Topodata2.Models.Home;
using Topodata2.Models.Mail;
using Topodata2.Models.UserFolder;
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
            return View(model ?? new List<OurTeamModel>());
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
                MailManager.SendMail(MailType.ContactUs, viewModel);
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

        public ActionResult Deslinde()
        {
            return View("Deslinde");
        }
        [HttpPost]
        public ActionResult Deslinde(DeslindeViewModel viewModel)
        {
            string message;
            viewModel.RegDate = TimePicker.GetLocalDateTime();
            if (!ModelState.IsValid)
            {
                message = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = message;
                return RedirectToAction("Deslinde", "Home");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            try
            {
                MailManager.SendMail(MailType.DeslinderRegistrationAdmin, viewModel);
                MailManager.SendMail(MailType.DeslinderRegistrationUser, viewModel);
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("Deslinde", "Home");
            }
            catch
            {
                // ignored
            }
            message = Messages.ErrorDesconocido;
            TempData["OperationMessage"] = message;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = message;
            return RedirectToAction("Deslinde", "Home");
        }

        public ActionResult Unsubscribe(string value)
        {
            var user = UserManager.GetUser(value);
            if (user == null || !user.Informed) return RedirectToAction("Index");
            UserManager.UpdateSubscribed(value, false);
            return View("SorryLeaving");
        }
    }
}