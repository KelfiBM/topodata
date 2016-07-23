using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Topodata2.Models;
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
                return new HttpStatusCodeResult(500);
            }
            UserSettingsManager.SaveUserSettingsNotification(viewModel);
            return View("Profile/ProfileSettings");
        }

        public ActionResult ProfileMain()
        {
            return View("Profile/ProfileMain");
        }

        [HttpPost]
        public ActionResult Login(UserViewModel userViewModel, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                if (UserManager.ValidateUser(userViewModel.Login, Response))
                {
                    return Redirect(returnUrl);
                }
                ModelState.AddModelError("", "");
            }
            return View("Register", userViewModel);
        }

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
                    new SubscribeViewModel().SendMessage(userViewModel.Register.Email);
                    FormsAuthentication.SetAuthCookie(userViewModel.Register.Username,false);
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