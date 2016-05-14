using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.Security;
using ModelMetadata = System.Web.Mvc.ModelMetadata;

namespace Topodata2.Models
{
    public class UserModels
    {

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
        [Remote("emailExist", "User", HttpMethod = "POST", ErrorMessage = "Este correo ya esta siendo usado")]
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
        [StringLength(255,MinimumLength = 6,ErrorMessage = "La contraseña debe tener minimo 6 caracteres")]
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

        [Mandatory(ErrorMessage = "Tienes que aceptar para continuar")]
        [Display(Name = "Acepto los terminos y condiciones")]
        [Range(typeof (bool), "true", "true", ErrorMessage = "Tienes que aceptar para continuar")]
        public bool TermsAndConditions { get; set; }

        [Display(Name = "Acepto recibir noticias Topodata")]
        [Range(typeof (bool), "false", "true")]
        public bool Informed { get; set; }
    }
    public class Mandatory : RequiredAttribute, IClientValidatable
    {

        public Mandatory()
        {

        }

        public override bool IsValid(object value)
        {

            if (value == null)
                return false;

            if (value is bool)
                return (bool)value;

            //Logic for other types goes here

            return false;

        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            return new ModelClientValidationRule[] { new ModelClientValidationRule() { ValidationType = "mandatory", ErrorMessage = this.ErrorMessage } };

        }

    }
}