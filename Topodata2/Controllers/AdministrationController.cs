using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Topodata2.Models;
using Topodata2.Models.Home;

namespace Topodata2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        // GET: Administration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HomeText()
        {
            return View(HomeManager.GetLastHomeText());
        }

        public ActionResult HomeSlide()
        {
            return View(HomeManager.GetCurrentHomeSlider());
        }

        [HttpPost]
        public ActionResult HomeText(HomeTextPrincipalViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("HomeText", "Administration");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (HomeManager.AddHomeText(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("HomeText", "Administration");
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("HomeText", "Administration");
        }

        [HttpPost]
        public ActionResult HomeSlide(HomeSliderViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("HomeSlide", "Administration");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (HomeManager.AddHomeSlideVideo(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("HomeSlide", "Administration");
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("HomeSlide", "Administration");
        }
    }
}