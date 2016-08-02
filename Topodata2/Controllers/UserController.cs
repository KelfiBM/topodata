using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Topodata2.Models;
using Topodata2.Models.Mail;
using Topodata2.Models.User;

namespace Topodata2.Controllers
{
    public class UserController : Controller
    {   
        // GET: User
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Register");
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Register");
        }

        [Authorize]
        public ActionResult ProfileSettings()
        {
            return View("Profile/ProfileSettings");
        }

        [Authorize]
        [HttpPost]
        public ActionResult ProfileSettingsNotification(UserProfileSettingsNotificationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["OperationStatus"] = "Error";
                return RedirectToAction("ProfileSettings");
            }
            if (UserSettingsManager.SaveNotification(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("ProfileSettings");
            }
            TempData["OperationStatus"] = "Error";
            return RedirectToAction("ProfileSettings");
        }

        [Authorize]
        [HttpPost]
        public ActionResult ProfileSettingsPassword(UserProfileSettingsPasswordViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("ProfileSettings");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (UserSettingsManager.SavePassword(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("ProfileSettings");
            }
            errorMessage = "La contraseña actual introducida es incorrecta";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("ProfileSettings");
        }

        [Authorize]
        [HttpPost]
        public ActionResult ProfileSettingsEditProfile(UserProfileSettingsEditProfileViewModel viewModel)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                errorMessage = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                TempData["OperationStatus"] = "Error";
                TempData["OperationMessage"] = errorMessage;
                return RedirectToAction("ProfileSettings");
                /*var errors = string.Join("; ",
                    ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage)));
                return Content("<script language='javascript' type='text/javascript'>alert('"+errors+"');</script>");*/
            }
            if (UserSettingsManager.SaveProfile(viewModel))
            {
                TempData["OperationStatus"] = "Success";
                return RedirectToAction("ProfileSettings");
            }
            errorMessage = "Ha sucedido un error desconocido, favor intentar mas tarde";
            TempData["OperationMessage"] = errorMessage;
            TempData["OperationStatus"] = "Error";
            ViewBag.OperationStatus = errorMessage;
            return RedirectToAction("ProfileSettings");
        }

        [Authorize]
        public ActionResult ProfileMain()
        {
            return View("Profile/ProfileMain");
        }

        [HttpPost]
        public ActionResult Login(UserViewModel userViewModel, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                return View("Register", userViewModel);
            }
            if (UserManager.ValidateUser(userViewModel.Login, Response))
            {
                return Redirect(returnUrl);
            }
            ModelState.AddModelError("", "");
            return View("Register", userViewModel);
        }

        [Authorize]
        public ActionResult Logout()
        {
            UserManager.Logout(Session,Response);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult EmailExists(UserViewModel email)
        {
            ItExists itExists = new ItExists();
            return Json(!itExists.ExistsCheck("email", email.Register.Email));
        }

        [HttpPost]
        public JsonResult UsernameExist(UserViewModel username)
        {
            ItExists itExists = new ItExists();
            return Json(!itExists.ExistsCheck("Username", username.Register.Username));
        }

        [HttpPost]
        public ActionResult Register(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                if (!ModelState.IsValidField("Register.Password"))
                {
                    userViewModel.Register.Password = "";
                    userViewModel.Register.ConfirmPassword = "";
                }
                if (!ModelState.IsValidField("Register.ConfirmPassword"))
                {
                    userViewModel.Register.ConfirmPassword = "";
                }
                return View(userViewModel);
            }
            else
            {
                if (userViewModel.Register.RegisterUser(userViewModel.Register))
                {

                    MailManager.SendRegistrationDone(new UserModel()
                    {
                        Email = userViewModel.Register.Email,
                        Informed = userViewModel.Register.Informed,
                        Name = userViewModel.Register.Name,
                        LastName = userViewModel.Register.LastName
                    });
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(userViewModel);
                }
            }

        }

        [HttpPost]
        public ActionResult Subscribe(SubscribeViewModel subscribeView)
        {
            if (ModelState.IsValid)
            {
                subscribeView.Subscribe(subscribeView);
                return Redirect(Request.UrlReferrer.ToString());
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public JsonResult emailExistsSubscribe(SubscribeViewModel subscribeView)
        {
            return Json(!new ItExists().ExistsCheckSuscribed(subscribeView.Email));
        }

    }
}