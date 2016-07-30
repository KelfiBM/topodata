using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Topodata2.Models;

namespace Topodata2.Controllers
{
    public class ServicesController : Controller
    {
        // GET: Services
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return View("Lockout");
            }
            else
            {
                return View("MustLogIn");
            }
        }

        [Route("Documento/{id:int}")]
        public ActionResult Document( int id = 0)
        {
            if (Request.IsAuthenticated)
            {
                var serviceDocument = new ServiceDocumentViewModel().GetDocumentById(id);
                if (serviceDocument.Exists)
                {
                    return View(serviceDocument);
                }
                else
                {
                    return RedirectToAction("NotFound","Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "User",new {returnUrl = Request.Url});
            }
        }

        public ActionResult Documents( int? page, int id = 0)
        {
            var serviceDocumentList = new ServiceDocumentViewModel().GetDocumentListByCategorie(id);
            if (serviceDocumentList != null && serviceDocumentList.Count > 0)
            {
                var pageNumber = page ?? 1;
                var onePage = serviceDocumentList.ToPagedList(pageNumber, 10);
                return View(onePage);
            }
            else
            {
                return RedirectToAction("Documents", "Services", new {id = 0});
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddDocument()
        {

            if (User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("NotFound", "Error");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddDocument(ServiceDocumentViewModel serviceDocument)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("AddDocument", "Services");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (serviceDocument.ImageUpload != null)
            {
                string[] validImageTypes =
                {
                        "image/gif",
                        "image/jpeg",
                        "image/pjpeg",
                        "image/png"
                    };

                if (!validImageTypes.Contains(serviceDocument.ImageUpload.ContentType))
                {
                    errorMessage = "Tiene que seleccionar una imagen de formato GIF, JPG o PNG";
                    /*ModelState.AddModelError("ImageUpload",
                        "Tiene que seleccionar una imagen de formato GIF, JPG o PNG");*/
                    return RedirectToAction("AddDocument", "Services");
                }
                else
                {
                    string uploadPath = "~/resources/img/documents";
                    string filename = serviceDocument.ImageUpload.FileName;
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
                    serviceDocument.ImageUpload.SaveAs(imagePath);
                    serviceDocument.ImagePath = imageUrl;
                }
            }
            else
            {
                serviceDocument.ImagePath = null;
            }
            if (serviceDocument.AddDocument(serviceDocument))
            {
                serviceDocument.SendAddedDocumentMessage();
                return RedirectToAction("Document", "Services", new { id = serviceDocument.GetLastDocummentAdded().Id });
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("AddDocument", "Services");


            if (ModelState.IsValid)
            {
                if (serviceDocument.ImageUpload != null)
                {
                    string[] validImageTypes =
                    {
                        "image/gif",
                        "image/jpeg",
                        "image/pjpeg",
                        "image/png"
                    };

                    if (!validImageTypes.Contains(serviceDocument.ImageUpload.ContentType))
                    {
                        ModelState.AddModelError("ImageUpload",
                            "Tiene que seleccionar una imagen de formato GIF, JPG o PNG");
                        return View(serviceDocument);
                    }
                    else
                    {
                        string uploadPath = "~/resources/img/documents";
                        string filename = serviceDocument.ImageUpload.FileName;
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
                        serviceDocument.ImageUpload.SaveAs(imagePath);
                        serviceDocument.ImagePath = imageUrl;
                    }
                }
                else
                {
                    serviceDocument.ImagePath = null;
                }
                if (serviceDocument.AddDocument(serviceDocument))
                {
                    serviceDocument.SendAddedDocumentMessage();
                    return RedirectToAction("Document", "Services", new { id = serviceDocument.GetLastDocummentAdded().Id });
                }
                return RedirectToAction("InternalServer", "Error");
            }
            return View(serviceDocument);
        }
    }
}