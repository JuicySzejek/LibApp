using System.ComponentModel.DataAnnotations;

namespace LibApp.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email address is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password address is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
