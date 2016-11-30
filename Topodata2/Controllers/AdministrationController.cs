using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Topodata2.Classes;
using Topodata2.Models;
using Topodata2.Models.Home;
using Topodata2.Models.Mail;
using Topodata2.Models.Service;
using Topodata2.Models.User;
using Topodata2.resources.Strings;

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
            return View(HomeManager.GetLastHomeTextViewModel());
        }

        public ActionResult HomeSlideVideo()
        {
            return View("HomeSlide/HomeSlideVideo",HomeManager.GetCurrentHomeSliderVideoViewModel());
        }

        public ActionResult OurTeam()
        {
            return View("OurTeam/OurTeam");
        }

        public ActionResult Flipboard()
        {
            return View("Flipboard/Flipboard");
        }

        public ActionResult ImageSeason()
        {
            return View("ImageSeason/ImageSeason");
        }

        [HttpPost]
        public ActionResult ImageSeason(HomeSliderImageSeasonViewModel viewModel)
        {
            var errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("ImageSeason", "Administration");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            try
            {
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
                            Messages.TieneFormatoImagen);
                        return View("ImageSeason/ImageSeason", viewModel);
                    }
                    const string uploadPath = "/resources/img/season";
                    const string filename = "season.jpg";
                    var imagePath = Path.Combine(Server.MapPath("~" + uploadPath), filename);
                    viewModel.ImageUpload.SaveAs(imagePath);
                    TempData["OperationStatus"] = "Success";
                    return RedirectToAction("ImageSeason", "Administration");
                }
            }
            catch
            {
                TempData["OperationMessage"] = errorMessage;
                TempData["OperationStatus"] = "Error";
                ViewBag.OperationStatus = errorMessage;
                return RedirectToAction("ImageSeason", "Administration");
            }
            
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("ImageSeason", "Administration");
        }

        [HttpPost]
        public ActionResult HomeText(TextoHomeViewModel viewModel)
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
        public ActionResult HomeSlideVideo(HomeSlideVideoViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("HomeSlideVideo", "Administration");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (HomeManager.AddHomeSlideVideo(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                viewModel.UrlVideo = Youtube.GetVideoId(viewModel.UrlVideo);
                /*var t1 = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;*/
                    new MailManager().SendMail(MailType.HomeVideoUpload, viewModel);
                /*});
                t1.Start();*/
                return RedirectToAction("HomeSlideVideo", "Administration");
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("HomeSlideVideo", "Administration");
        }

        [HttpPost]
        public ActionResult AddOurTeam(OurTeamViewModel viewModel)
        {
            var errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
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
                        Messages.TieneFormatoImagen);
                    return View("OurTeam/OurTeam",viewModel);
                }
                const string uploadPath = "~/resources/img/team";
                var filename = viewModel.ImageUpload.FileName;
                var imagePath = Path.Combine(Server.MapPath(uploadPath), filename);
                var tempFileName = filename;
                if (System.IO.File.Exists(imagePath))
                {
                    var counter = 1;
                    while (System.IO.File.Exists(imagePath))
                    {
                        tempFileName = counter + filename;
                        imagePath = Path.Combine(Server.MapPath(uploadPath), tempFileName);
                        counter++;
                    }
                    filename = tempFileName;
                }
                var imageUrl = uploadPath + "/" + filename;
                imageUrl = imageUrl.Substring(1, imageUrl.Length - 1);
                viewModel.ImageUpload.SaveAs(imagePath);
                viewModel.ImagePath = imageUrl;
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
            
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("OurTeam", "Administration");
        }

        public ActionResult DeleteOurTeam(int id)
        {
            var model = new OurTeam
            {
                Id = Convert.ToInt32(id)
            };

            if (HomeManager.DeleteOurTeam(model))
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
            var model = new Flipboard
            {
                Id = Convert.ToInt32(id)
            };

            if (HomeManager.DeleteFlipboard(model))
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

        public ActionResult Documents()
        {
            return View("Documents/Documents");
        }

        public ActionResult AddDocument()
        {
            return View("AddDocument");
        }

        public ActionResult DeleteDocument(int id)
        {
            if (ServiceManager.DeleteDocument(id))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("Documents", "Administration");
            }
            var errorMessage = string.Join("; ",
                ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
            TempData["OperationStatus"] = "Error";
            TempData["OperationMessage"] = errorMessage;
            return RedirectToAction("Documents", "Administration");
        }

        [HttpPost]
        public ActionResult AddDocument(DocumentViewModel model)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return View("Documents/Documents", model);
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (model.ImageUpload != null)
            {
                string[] validImageTypes =
                {
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };

                if (!validImageTypes.Contains(model.ImageUpload.ContentType))
                {
                    errorMessage = "Tiene que seleccionar una imagen de formato GIF, JPG o PNG";
                    TempData["OperationStatus"] = "Error";
                    TempData["OperationMessage"] = errorMessage;

                    ModelState.AddModelError("ImageUpload",
                        Messages.TieneFormatoImagen);
                    return View("Documents/Documents",model);
                }
                const string uploadPath = "~/resources/img/documents";
                var filename = model.ImageUpload.FileName;
                var imagePath = Path.Combine(Server.MapPath(uploadPath), filename);
                var tempFileName = filename;
                if (System.IO.File.Exists(imagePath))
                {
                    var counter = 1;
                    while (System.IO.File.Exists(imagePath))
                    {
                        tempFileName = counter + filename;
                        imagePath = Path.Combine(Server.MapPath(uploadPath), tempFileName);
                        counter++;
                    }
                    filename = tempFileName;
                }
                var imageUrl = uploadPath + "/" + filename;
                imageUrl = imageUrl.Substring(1, imageUrl.Length - 1);
                model.ImageUpload.SaveAs(imagePath);
                model.ImagePath = imageUrl;
            }
            else
            {
                model.ImagePath = null;
            }
            if (ServiceManager.AddDocument(model))
            {
                TempData["OperationStatus"] = "Success";
                var lastDocument = ServiceManager.GetLastDocument();
                new MailManager().SendMail(MailType.NewDocumentMessage, lastDocument);
                return RedirectToAction("Document", "Services", new { id = lastDocument.Id });
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return View("Documents/Documents", model);
        }

        [HttpPost]
        public ActionResult GetContenidoBySubCategorie(int subCategorie)
        {

            var contenido = new SelectList(
                ServiceManager.GetAllContenidoBySubcategorieId(
                    new SubCategorieModel {Id = subCategorie}),
                "Id", "Descripcion");
            return Json(contenido);
        }

        public ActionResult Sectores()
        {
            return View("Sectores/Sectores");
        }

        public ActionResult DeleteSubcategorie(int id)
        {
            if (ServiceManager.DeleteSubCategorie(id))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("Sectores", "Administration");
            }
            var errorMessage = string.Join("; ",
                ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
            TempData["OperationStatus"] = "Error";
            TempData["OperationMessage"] = errorMessage;
            return RedirectToAction("Sectores", "Administration");
        }

        [HttpPost]
        public ActionResult EditSubCategorie(SubCategorieViewModel model)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("Sectores", "Administration");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (ServiceManager.EditCategorie(model.IdSubCategoria,model.Descripcion))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("Sectores", "Administration");
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("Sectores", "Administration");
        }
    }
}