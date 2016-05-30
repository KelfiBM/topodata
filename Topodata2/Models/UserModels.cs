﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using ModelMetadata = System.Web.Mvc.ModelMetadata;

namespace Topodata2.Models
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Mantenerme Conectado")]
        public bool KeepConnected { get; set; }

        public bool IsValid(string username, string password)
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string query = "SELECT [Username] FROM [Users] WHERE [Username] = @u AND [Password] = @p";
                var com = new SqlCommand(query,con);
                com.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = username;
                com.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar))
                    .Value = password;
                con.Open();
                var reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    com.Dispose();
                    con.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    com.Dispose();
                    con.Dispose();
                    return false;
                }

            }
        }
    }

    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Nombre")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Apellido")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Email")]
        [Remote("emailExists", "User", HttpMethod = "POST", ErrorMessage = "Este correo ya esta siendo usado")]
        [EmailAddress(ErrorMessage = "Correo invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Text)]
        [StringLength(30,
            MinimumLength = 5,
            ErrorMessage = "El nombre de usuario debe tener minimo 5 caracteres y maximo 30")]
        [Remote("usernameExist", "User", HttpMethod = "POST",
            ErrorMessage = "Este nombre de usuario esta siendo usado")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener minimo 6 caracteres")]
        [MembershipPassword(
            MinRequiredNonAlphanumericCharacters = 0,
            MinRequiredPasswordLength = 6,
            MinPasswordLengthError = "La contraseña debe tener minimo 6 caracteres"
            )]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessage = "Este campo debe coincidir con el campo contraseña")]
        [MembershipPassword(
            MinRequiredNonAlphanumericCharacters = 0,
            MinRequiredPasswordLength = 6,
            MinPasswordLengthError = "La contraseña debe tener minimo 6 caracteres"
            )]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }

        [MustBeTrue(ErrorMessage = "Debe aceptar los terminos y condiciones")]
        [Display(Name = "Acepto los terminos y condiciones")]
        public bool TermsAndConditions { get; set; }

        [Display(Name = "Acepto recibir noticias Topodata")]
        public bool Informed { get; set; }

        public bool RegisterUser(RegisterUserViewModel registerUserView)
        {
            try
            {
                string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    string query =
                        "INSERT INTO [Topodata].[dbo].[Users] VALUES (@name, @lastName, @email, @username, @password, @informed, @regDate)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("name", registerUserView.Name);
                    sqlCommand.Parameters.AddWithValue("lastName", registerUserView.LastName);
                    sqlCommand.Parameters.AddWithValue("email", registerUserView.Email);
                    sqlCommand.Parameters.AddWithValue("username", registerUserView.Username);
                    sqlCommand.Parameters.AddWithValue("password", registerUserView.Password);
                    sqlCommand.Parameters.AddWithValue("informed", registerUserView.Informed);
                    sqlCommand.Parameters.AddWithValue("regDate", DateTime.Now);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }
    }

    public class SubscribeViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Correo Electronico")]
        [EmailAddress(ErrorMessage = "Correo invalido")]
        [Remote("emailExistsSubscribe", "User", HttpMethod = "POST", ErrorMessage = "Este correo ya esta suscrito")]
        public string Email { get; set; }

        public bool Subscribe(SubscribeViewModel subscribeView)
        {
            try
            {
                string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    string query = "INSERT INTO [Topodata].[dbo].[Suscrito] VALUES (@email, @informed)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("email", subscribeView.Email);
                    sqlCommand.Parameters.AddWithValue("informed", 1);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConnection.Close();
                }
                SendMessage(subscribeView.Email);
                return true;
            }
            catch (Exception es)
            {
                return false;
            }
        }

        private bool SendMessage(string toMail)
        {
            try
            {
                const string from = "info@topodata.com";
                string to = toMail;
                const string subject = "Gracias por ser parte de Topodata!";
                const string pass = "Topo.1953";

                string body = "<b>Gracias por unirte la comunidad Topodata</b><br/>";
                body += "Esperemos que te sientas comodo perteneciendo a nuestra comunidad.<br />";
                body += "Con esto estaras al tanto de lo que sucede en Topodata y asi mantenerte al dia de los ultimos acontecimientos que esta te ofrece.<br />";
                body += "<br />Topodata";

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from, "Topodata");
                mailMessage.To.Add(new MailAddress(to));
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Credentials = new NetworkCredential(from,pass);
                smtpClient.Send(mailMessage);
                return true;
            }
           catch (Exception)
            {

                return false;
            }
        }
    }

    public class MustBeTrueAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if(value.GetType() != typeof(bool)) throw new InvalidOperationException("Must be boolean");
            return (bool) value == true;
            
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            yield return new ModelClientValidationRule()
            {
                ErrorMessage = "Debe seleccionar este campo",
                ValidationType = "mustbetrue"
            };
        }
    }

    public class ItExists
    {
        private string connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        public bool ExistsCheck(string attrib, string data)
        {

            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand com = new SqlCommand();
                SqlDataReader reader;
                com.CommandText = string.Format("SELECT * FROM [Users] WHERE " + attrib + " = '{0}'", data);
                com.CommandType = CommandType.Text;
                com.Connection = con;

                con.Open();
                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    com.Dispose();
                    con.Close();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    com.Dispose();
                    con.Close();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ExistsCheckSuscribed(string email)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand com = new SqlCommand();
                SqlDataReader reader;
                com.CommandText = string.Format("SELECT * FROM [Suscrito] WHERE Email = '{0}'", email);
                com.CommandType = CommandType.Text;
                com.Connection = con;

                con.Open();
                reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    com.Dispose();
                    con.Close();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    com.Dispose();
                    con.Close();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}