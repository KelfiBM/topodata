using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;


namespace Topodata2.Models.User
{
    public class UserProfileSettings
    {
        
    }
    public class UserProfileSettingsNotificationViewModel
    {
        [Display(Name = "Notificarme por correo sobre los documentos nuevos en TOPODATA")]
        public bool NewDocuments { get; set; }
    }

    public class UserProfileSettingsPasswordViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string PasswordOld { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener minimo 6 caracteres")]
        [MembershipPassword(
            MinRequiredNonAlphanumericCharacters = 0,
            MinRequiredPasswordLength = 6,
            MinPasswordLengthError = "La contraseña debe tener minimo 6 caracteres"
            )]
        [Display(Name = "Contraseña Nueva")]
        public string PasswordNew { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Compare("PasswordNew",ErrorMessage = "Este campo debe coincidir con el campo anterior")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string PasswordNewConfirm { get; set; }
    }

    public class UserProfileSettingsEditProfileViewModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Nombre:")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Apellido:")]
        [DataType(DataType.Text)]
        public string Lastname { get; set; }

        [Display(Name = "Correo:")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nombre de Usuario:")]
        public string Username { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Registrado desde:")]
        public DateTime RegDate { get; set; }

    }
}