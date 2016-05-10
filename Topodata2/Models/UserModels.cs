using System.ComponentModel.DataAnnotations;
using System.Web.Security;

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
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Text)]
        [StringLength(30, MinimumLength = 5)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [MembershipPassword]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [MembershipPassword]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Would you like to receive news about Topodata?")]
        [Range(typeof(bool), "false", "true")]
        public bool Informed { get; set; }
    }

}