using System.Linq;
using System.Web.Mvc;
using Topodata2.Models;
using Topodata2.Models.Mail;
using Topodata2.Models.UserFolder;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using Topodata2.Managers;
using Topodata2.Models.Service;
using Topodata2.ViewModels;

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

        
        public ActionResult ProfileSettings(string tab = "")
        {
            var returnUrl = Request.Url + "#" + tab;
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Index", new {returnUrl});

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
            ModelState.AddModelError("", "El usuario y/o contraseña son incorrectos");
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
            var recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (string.IsNullOrEmpty(recaptchaHelper.Response))
            {
                ModelState.AddModelError("", "Comprueba que no eres un robot.");
                return View(userViewModel);
            }

            var recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                ModelState.AddModelError("", "Respuesta incorrecta.");
            }
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
            if (!userViewModel.Register.RegisterUser(userViewModel.Register)) return View(userViewModel);
            var model = new UserModel
            {
                Email = userViewModel.Register.Email,
                Informed = userViewModel.Register.Informed,
                LastName = userViewModel.Register.LastName,
                Name = userViewModel.Register.Name,
                Password = userViewModel.Register.Password,
                Username = userViewModel.Register.Username
            };
            MailManager.SendMail(MailType.RegistrationDoneUser,model);
            MailManager.SendMail(MailType.RegistrationDoneAdmin, model);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Subscribe(SubscribeViewModel subscribeView)
        {
            if (!ModelState.IsValid) return Redirect(Request.UrlReferrer?.ToString());
            new SuscritoService().Insert(subscribeView);
            MailManager.SendMail(MailType.SubscribeDone, subscribeView.Email);
            return Redirect(Request.UrlReferrer?.ToString());
        }

        [HttpPost]
        public JsonResult emailExistsSubscribe(SubscribeViewModel subscribeView)
        {
            return Json(!new ItExists().ExistsCheckSuscribed(subscribeView.Email));
        }

    }
}