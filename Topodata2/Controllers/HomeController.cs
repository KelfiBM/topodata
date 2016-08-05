using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Topodata2.Models;
using Topodata2.Models.Home;
using Topodata2.Models.Mail;

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
            return View(HomeManager.GetAllOurTeam());
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
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactUsViewModel contactUs)
        {
            if (ModelState.IsValid)
            {
                if (contactUs.SendMessage(contactUs))
                {
                    TempData["Success"] = "Tu mensaje ha sido enviado con exito";
                    return RedirectToAction("Index");
                }
                ViewData["Error"] = "Ha sucedido un problema enviando tu mensaje, Intenta mas tarde ";
                return View(contactUs);
            }
            return View(contactUs);

        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Deslinder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Deslinder(DeslinderViewModel viewModel)
        {
            string errorMessage;
            viewModel.RegDate = System.DateTime.Now;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("Deslinder", "Home");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (MailManager.SendDeslinderRegistrationAdmin(viewModel))
            {
                if (MailManager.SendDeslinderRegistrationUser(viewModel))
                {
                    TempData["OperationStatus"] = "Success";
                    return RedirectToAction("Deslinder", "Home");
                }
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("Deslinder", "Home");
        }
    }
}