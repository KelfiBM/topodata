using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Topodata2.Models;

namespace Topodata2.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterUserViewModel registerUser)
        {
            if (ModelState.IsValid)
            {
                string connection = @"Data Source=KELFI-PC\SQLINSTANCE;Initial Catalog=Topodata;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    string query =
                        "INSERT INTO [Topodata].[dbo].[Users] VALUES (@name, @lastName, @email, @username, @password, @informed, @regDate)";
                    SqlCommand sqlCommand = new SqlCommand(query,sqlConnection);
                    sqlCommand.Parameters.AddWithValue("name", registerUser.Name);
                    sqlCommand.Parameters.AddWithValue("lastName", registerUser.LastName);
                    sqlCommand.Parameters.AddWithValue("email", registerUser.Email);
                    sqlCommand.Parameters.AddWithValue("username", registerUser.Username);
                    sqlCommand.Parameters.AddWithValue("password", registerUser.Password);
                    sqlCommand.Parameters.AddWithValue("informed", registerUser.Informed);
                    sqlCommand.Parameters.AddWithValue("regDate", DateTime.Now);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
            return View();
        }

    }
}