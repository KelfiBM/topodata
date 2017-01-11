using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        [HttpPost]
        public ContentResult GetContentForTable(int index)
        {
            var type = (ActionType) index;
            string result = null;
            switch (type)
            {
                case ActionType.Documents:
                    result =
                        JsonConvert.SerializeObject(
                            ServiceManager.GetLastDocumentsAdded().Select(documento => new AllDocumentsViewModel
                            {
                                Id = documento.Id,
                                Nombre = documento.Nombre,
                                Descripcion = documento.Descripcion,
                                SubCategoria = documento.SubCategoria,
                                Contenido = documento.Contenido,
                                RegDate = documento.RegDate.ToShortDateString(),
                                Url = documento.Url
                            }).ToList());
                    break;
                case ActionType.HomeSlideVideo:
                    break;
                case ActionType.Flipboard:
                    result =
                        JsonConvert.SerializeObject(
                            HomeManager.GetAllFlipboard().Select(flipboard => new AllFlipboardViewModel
                            {
                                Id = flipboard.Id,
                                Name = flipboard.Name,
                                RegDate = flipboard.RegDate.ToShortDateString(),
                                Url = flipboard.Url
                            }).ToList());
                    break;
                case ActionType.ImageSeason:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Content(result, "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult DeleteContent(int[] ids, int index)
        {
            var type = (ActionType) index;
            var allGood = new List<bool>
            {
                false
            };
            switch (type)
            {
                case ActionType.Documents:
                    allGood = ids.Select(ServiceManager.DeleteDocument).ToList();
                    break;
                case ActionType.HomeSlideVideo:
                    break;
                case ActionType.Flipboard:
                    allGood = ids.Select(HomeManager.DeleteFlipboard).ToList();
                    break;
                case ActionType.ImageSeason:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return PartialView(allGood.Any(good => !good) ? "_AlertError" : "_AlertSuccess");
        }

        //---------------------------------------------------//
        public ActionResult Documents(string opStatus = null, string opMessage = null)
        {
            if (opStatus != null)
            {
                TempData["OperationStatus"] = opStatus;
                TempData["OperationMessage"] = opMessage;
            }

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
            return View("Administration", model);
        }

        [HttpPost]
        public ActionResult Documents(DocumentViewModel model)
        {
            string errorMessage = null;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ", ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return RedirectToAction("Documents", new {opStatus = General.OpStatusError, opMessage = errorMessage});
            }
            if (model.ImageUpload != null)
            {
                string[] validImageTypes =
                {
                    "image/gif", "image/jpeg", "image/pjpeg", "image/png"
                };

                if (!validImageTypes.Contains(model.ImageUpload.ContentType))
                {
                    errorMessage = Messages.TieneFormatoImagen;
                    ModelState.AddModelError("ImageUpload", Messages.TieneFormatoImagen);
                    return RedirectToAction("Documents", new { opStatus = General.OpStatusError, opMessage = errorMessage });
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
                TempData["OperationStatus"] = General.OpStatusSuccess;
                var lastDocument = ServiceManager.GetLastDocument();
                MailManager.SendMail(MailType.NewDocumentMessage, lastDocument);
                return RedirectToAction("Document", "Services", new {id = lastDocument.Id});
            }
            errorMessage = Messages.ErrorDesconocido;
            return RedirectToAction("Documents", new { opStatus = General.OpStatusError, opMessage = errorMessage });
        }

        [HttpPost]
        public ActionResult GetContenidoBySubCategorie(int subCategorie)
        {
            var contenido = new SelectList(ServiceManager.GetAllContenidoBySubcategorieId(new SubCategorieModel {Id = subCategorie}), "Id", "Descripcion");
            return Json(contenido);
        }

        //----------------------------------------------------//
        public ActionResult HomeSlideVideo(string opStatus = null, string opMessage = null)
        {
            if (opStatus != null)
            {
                TempData["OperationStatus"] = opStatus;
                TempData["OperationMessage"] = opMessage;
            }
            var model = new AdministrationModel
            {
                Action = ActionType.HomeSlideVideo,
                Title = "Video Slide",
                UseTable = false,
                UseTextArea = false,
                IdTabPrincipal = "add",
                IdTable = null,
                UrlDeleteRecord = null,
                IdsFormValidation = new List<string> {"AddHomeSlideVideoForm"},
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
                errorMessage = string.Join("; ", ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return RedirectToAction("HomeSlideVideo",
                    new {opStatus = General.OpStatusError, opMessage = errorMessage});
            }
            if (HomeManager.AddHomeSlideVideo(viewModel))
            {
                viewModel.UrlVideo = Youtube.GetVideoId(viewModel.UrlVideo);
                MailManager.SendMail(MailType.HomeVideoUpload, viewModel);
                return RedirectToAction("HomeSlideVideo",
                    new {opStatus = General.OpStatusSuccess, opMessage = (string) null});
            }
            errorMessage = Messages.ErrorDesconocido;
            return RedirectToAction("HomeSlideVideo", new {opStatus = General.OpStatusError, opMessage = errorMessage});
        }

        //-----------------------------------------------------//
        public ActionResult Flipboard(string opStatus = null, string opMessage = null)
        {
            if (opStatus != null)
            {
                TempData["OperationStatus"] = opStatus;
                TempData["OperationMessage"] = opMessage;
            }
            var model = new AdministrationModel
            {
                Action = ActionType.Flipboard,
                Title = "Flipboard",
                UseTable = true,
                UseTextArea = false,
                IdTabPrincipal = "add",
                IdTable = "allFlipboardTable",
                UrlDeleteRecord = "/Administration/DeleteFlipboard/",
                IdsFormValidation = new List<string> {"addFlipboardForm"},
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

            return View("Administration", model);
        }
        
        [HttpPost]
        public ActionResult Flipboard(FlipboardViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ", ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return RedirectToAction("Flipboard", new { opStatus = General.OpStatusError, opMessage = errorMessage });
            }
            if (HomeManager.AddFlipboard(viewModel))
            {
                return RedirectToAction("Flipboard", new {opStatus = General.OpStatusSuccess, opMessage = (string) null});
            }
            errorMessage = Messages.ErrorDesconocido;
            return RedirectToAction("Flipboard", new { opStatus = General.OpStatusError, opMessage = errorMessage });
        }

        //-----------------------------------------------------//
        public ActionResult ImageSeason(string opStatus = null, string opMessage = null)
        {
            if (opStatus != null)
            {
                TempData["OperationStatus"] = opStatus;
                TempData["OperationMessage"] = opMessage;
            }
            var model = new AdministrationModel
            {
                Action = ActionType.ImageSeason,
                Title = "Imagen Slide",
                UseTable = false,
                UseTextArea = false,
                IdTabPrincipal = "add",
                IdTable = null,
                UrlDeleteRecord = null,
                IdsFormValidation = new List<string> {"addImageSeasonForm"},
                Tabs = new List<ThreeValuesString>
                {
                    new ThreeValuesString
                    {
                        Key = "add",
                        Value1 = "Añadir Imagen Slide",
                        Value2 = "ImageSeason/_AddImageSeason"
                    },
                },
                ViewModel = new HomeSliderImageSeasonViewModel(),
                IdHiddenRaw = null,
                UseDetailFormatter = false
            };
            return View("Administration", model);
        }

        [HttpPost]
        public ActionResult ImageSeason(HomeSliderImageSeasonViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ", ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return RedirectToAction("ImageSeason", new {opStatus = General.OpStatusError, opMessage = errorMessage});
            }

            if (viewModel.ImageUpload != null)
            {
                string[] validImageTypes =
                {
                    "image/jpeg"
                };

                if (!validImageTypes.Contains(viewModel.ImageUpload.ContentType))
                {
                    ModelState.AddModelError("ImageUpload", Messages.TieneFormatoJPG);
                    errorMessage = Messages.TieneFormatoJPG;
                    return RedirectToAction("ImageSeason",
                        new {opStatus = General.OpStatusError, opMessage = errorMessage});
                }
                var imagePath = Path.Combine(Server.MapPath("~/resources/img/season"), "season.jpg");
                viewModel.ImageUpload.SaveAs(imagePath);
                return RedirectToAction("ImageSeason",
                    new {opStatus = General.OpStatusSuccess, opMessage = (string) null});
            }
            errorMessage = Messages.ErrorDesconocido;
            return RedirectToAction("ImageSeason", new {opStatus = General.OpStatusError, opMessage = errorMessage});
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
            var errorMessage = string.Join("; ", ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
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
                errorMessage = string.Join("; ", ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
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