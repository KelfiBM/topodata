using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Topodata2.Models;
using Topodata2.Models.Home;
using Topodata2.Models.User;

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

        public ActionResult OurTeam()
        {
            return View("OurTeam/OurTeam");
        }

        public ActionResult Flipboard()
        {
            return View("Flipboard/Flipboard");
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

        [HttpPost]
        public ActionResult AddOurTeam(OurTeamViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("OurTeam", "Administration");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (viewModel.ImageUpload != null)
            {
                string[] validImageTypes =
                {
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };

                if (!validImageTypes.Contains(viewModel.ImageUpload.ContentType))
                {
                    ModelState.AddModelError("ImageUpload",
                        "Tiene que seleccionar una imagen de formato GIF, JPG o PNG");
                    return View("OurTeam/OurTeam",viewModel);
                }
                else
                {
                    string uploadPath = "~/resources/img/team";
                    string filename = viewModel.ImageUpload.FileName;
                    string imagePath = Path.Combine(Server.MapPath(uploadPath), filename);
                    string tempFileName = filename;
                    if (System.IO.File.Exists(imagePath))
                    {
                        int counter = 1;
                        while (System.IO.File.Exists(imagePath))
                        {
                            tempFileName = counter.ToString() + filename;
                            imagePath = Path.Combine(Server.MapPath(uploadPath), tempFileName);
                            counter++;
                        }
                        filename = tempFileName;
                    }
                    string imageUrl = uploadPath + "/" + filename;
                    imageUrl = imageUrl.Substring(1, imageUrl.Length - 1);
                    viewModel.ImageUpload.SaveAs(imagePath);
                    viewModel.ImagePath = imageUrl;
                }
            }
            else
            {
                viewModel.ImagePath = null;
            }
            if (HomeManager.AddOurTeam(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("OurTeam", "Administration");
            }
            return RedirectToAction("InternalServer", "Error");
            if (HomeManager.AddOurTeam(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("OurTeam", "Administration");
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("OurTeam", "Administration");
        }

        public ActionResult DeleteOurTeam(int id)
        {
            OurTeamViewModel viewModel = new OurTeamViewModel()
            {
                IdOurTeam = Convert.ToInt32(id)
            };

            if (HomeManager.DeleteOurTeam(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("OurTeam", "Administration");
            }
            var errorMessage = string.Join("; ",
                ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
            TempData["OperationStatus"] = "Error";
            TempData["OperationMessage"] = errorMessage;
            return RedirectToAction("OurTeam", "Administration");

        }

        [HttpPost]
        public ActionResult AddFlipboard(FlipboardViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("Flipboard", "Administration");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (HomeManager.AddFlipboard(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("Flipboard", "Administration");
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("Flipboard", "Administration");
        }

        public ActionResult DeleteFlipboard(int id)
        {
            FlipboardViewModel viewModel = new FlipboardViewModel()
            {
                IdFlipboard = Convert.ToInt32(id)
            };

            if (HomeManager.DeleteFlipboard(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("Flipboard", "Administration");
            }
            var errorMessage = string.Join("; ",
                ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
            TempData["OperationStatus"] = "Error";
            TempData["OperationMessage"] = errorMessage;
            return RedirectToAction("Flipboard", "Administration");
        }

        public ActionResult AllUser()
        {
            return View("Users/AllUsers");
        }
        public ActionResult DeleteUser(int id)
        {
            var viewModel = new UserModel()
            {
                IdUser = Convert.ToInt32(id)
            };

            if (UserManager.DeleteUser(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("AllUser", "Administration");
            }
            var errorMessage = string.Join("; ",
                ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
            TempData["OperationStatus"] = "Error";
            TempData["OperationMessage"] = errorMessage;
            return RedirectToAction("AllUser", "Administration");

        }
    }
}