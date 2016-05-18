using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Topodata2.Models;

namespace Topodata2.Controllers
{
    public class UserController : Controller
    {
        private string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: User
        public ActionResult Register()
        {
            return View();
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
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    string query =
                        "INSERT INTO [Topodata].[dbo].[Users] VALUES (@name, @lastName, @email, @username, @password, @informed, @regDate)";
                    SqlCommand sqlCommand = new SqlCommand(query,sqlConnection);
                    sqlCommand.Parameters.AddWithValue("name", userViewModel.Register.Name);
                    sqlCommand.Parameters.AddWithValue("lastName", userViewModel.Register.LastName);
                    sqlCommand.Parameters.AddWithValue("email", userViewModel.Register.Email);
                    sqlCommand.Parameters.AddWithValue("username", userViewModel.Register.Username);
                    sqlCommand.Parameters.AddWithValue("password", userViewModel.Register.Password);
                    sqlCommand.Parameters.AddWithValue("informed", userViewModel.Register.Informed);
                    sqlCommand.Parameters.AddWithValue("regDate", DateTime.Now);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                return RedirectToAction("Index", "Home");
            }

        }

    }
}