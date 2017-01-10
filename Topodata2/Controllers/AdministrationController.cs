using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Topodata2.Classes;
using Topodata2.Managers;
using Topodata2.Models;
using Topodata2.Models.Administration;
using Topodata2.Models.Home;
using Topodata2.Models.Mail;
using Topodata2.Models.Service;
using Topodata2.Models.Shared;
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

        public ActionResult OurTeam()
        {
            return View("OurTeam/OurTeam");
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
                    return View("OurTeam/OurTeam", viewModel);
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

        //---------------------------------------------------//
        public ActionResult Documents()
        {
            var model = new AdministrationModel
            {
                Action = ActionType.Documents,
                Title = "Documentos",
                UseTable = true,
                UseTextArea = true,
                IdTabPrincipal = "add",
                IdTable = "allDocumentsTable",
                UrlDeleteRecord = "/Administration/DeleteDocument/",
                IdsFormValidation = new List<string> {"AddDocumentForm"},
                Tabs = new List<ThreeValuesString>
                {
                    new ThreeValuesString
                    {
                        Key = "add",
                        Value1 = "Añadir Documento",
                        Value2 = "Documents/_AddDocument"
                    },
                    new ThreeValuesString
                    {
                        Key = "all",
                        Value1 = "Todos los Documentos",
                        Value2 = "Documents/_AllDocuments"
                    }
                },
                ViewModel = new DocumentViewModel(),
                IdHiddenRaw = "Descripcion",
                UseDetailFormatter = true
            };
            return View("Administration",model);
        }

        public ContentResult GetAllDocumentTable()
        {
            var documentos = ServiceManager.GetLastDocumentsAdded();
            var result = documentos.Select(documento => new AllDocumentsViewModel
            {
                Id = documento.Id,
                Nombre = documento.Nombre,
                Descripcion = documento.Descripcion,
                SubCategoria = documento.SubCategoria,
                Contenido = documento.Contenido,
                RegDate = documento.RegDate.ToShortDateString(),
                Url = documento.Url
            }).ToList();
            return Content(JsonConvert.SerializeObject(result), "application/json",
                Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult DeleteDocument(int[] ids)
        {
            var allGood = ids.Select(ServiceManager.DeleteDocument).ToList();
            return PartialView(allGood.Any(good => !good) ? "_AlertError" : "_AlertSuccess");
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
                    "image/jpeg"
                };

                if (!validImageTypes.Contains(model.ImageUpload.ContentType))
                {
                    errorMessage = "Tiene que seleccionar una imagen de formato JPG";
                    TempData["OperationStatus"] = "Error";
                    TempData["OperationMessage"] = errorMessage;

                    ModelState.AddModelError("ImageUpload",
                        Messages.TieneFormatoImagen);
                    return View("Documents/Documents", model);
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
                MailManager.SendMail(MailType.NewDocumentMessage, lastDocument);
                return RedirectToAction("Document", "Services", new {id = lastDocument.Id});
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
                    new SubCategorieModel { Id = subCategorie }),
                "Id", "Descripcion");
            return Json(contenido);
        }
        //----------------------------------------------------//
        public ActionResult HomeSlideVideo()
        {
            var model = new AdministrationModel
            {
                Action = ActionType.HomeSlideVideo,
                Title = "Video Slide",
                UseTable = false,
                UseTextArea = false,
                IdTabPrincipal = "add",
                IdTable = null,
                UrlDeleteRecord = null,
                IdsFormValidation = new List<string> { "AddHomeSlideVideoForm" },
                Tabs = new List<ThreeValuesString>
                {
                    new ThreeValuesString
                    {
                        Key = "add",
                        Value1 = "Añadir Video",
                        Value2 = "HomeSlide/_AddHomeSlideVideo"
                    },
                },
                ViewModel = HomeManager.GetCurrentHomeSliderVideoViewModel(),
                IdHiddenRaw = null,
                UseDetailFormatter = false
            };

            return View("Administration", model);
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
                MailManager.SendMail(MailType.HomeVideoUpload, viewModel);
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
        //-----------------------------------------------------//
        public ActionResult Flipboard()
        {
            var model = new AdministrationModel
            {
                Action = ActionType.Flipboard,
                Title = "Flipboard",
                UseTable = true,
                UseTextArea = false,
                IdTabPrincipal = "add",
                IdTable = "allFlipboardTable",
                UrlDeleteRecord = "/Administration/DeleteFlipboard/",
                IdsFormValidation = new List<string> { "addFlipboardForm" },
                Tabs = new List<ThreeValuesString>
                {
                    new ThreeValuesString
                    {
                        Key = "add",
                        Value1 = "Añadir Revista",
                        Value2 = "Flipboard/_AddFlipboard"
                    },
                    new ThreeValuesString
                    {
                        Key = "all",
                        Value1 = "Todas las Revistas",
                        Value2 = "Flipboard/_DeleteFlipboard"
                    }
                },
                ViewModel = new FlipboardViewModel(),
                IdHiddenRaw = null,
                UseDetailFormatter = true
            };

            return View("Administration",model);
        }

        public ContentResult GetAllFlipboardTable()
        {
            var flipboards = HomeManager.GetAllFlipboard();
            var result = flipboards.Select(flipboard => new AllFlipboardViewModel
            {
                Id = flipboard.Id,
                Name = flipboard.Name,
                RegDate = flipboard.RegDate.ToShortDateString(),
                Url = flipboard.Url
            }).ToList();
            return Content(JsonConvert.SerializeObject(result), "application/json",
                Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult DeleteFlipboard(int[] ids)
        {
            var allGood = ids.Select(HomeManager.DeleteFlipboard).ToList();
            return PartialView(allGood.Any(good => !good) ? "_AlertError" : "_AlertSuccess");
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

        //-----------------------------------------------------//
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
            if (ServiceManager.EditCategorie(model.IdSubCategoria, model.Descripcion))
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