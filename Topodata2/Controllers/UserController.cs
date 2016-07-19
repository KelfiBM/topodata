using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Topodata2.Models;
using Topodata2.Models.User;

namespace Topodata2.Controllers
{
    public class UserController : Controller
    {
        private string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
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

        public ActionResult ProfileSettings()
        {
            return View("Profile/ProfileSettings");
        }

        [HttpPost]
        public ActionResult ProfileSettingsNotification(UserProfileSettingsNotificationViewModel viewModel)
        {

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
                if (new CustomMerbershipProvider().ValidateUser(userViewModel.Login.Username, userViewModel.Login.Password))
                {
                    FormsAuthentication.SetAuthCookie(userViewModel.Login.Username, userViewModel.Login.KeepConnected);
                    return Redirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("","");
                }
            }
            return View("Register",userViewModel);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult emailExists(UserViewModel Email)
        {
            ItExists itExists = new ItExists();
            return Json(!itExists.ExistsCheck("Email", Email.Register.Email));
        }

        [HttpPost]
        public JsonResult usernameExist(UserViewModel username)
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