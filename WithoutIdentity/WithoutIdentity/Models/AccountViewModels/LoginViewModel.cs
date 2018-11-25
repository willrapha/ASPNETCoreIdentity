using System.ComponentModel.DataAnnotations;

namespace WithoutIdentity.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Lembrar Login")]
        public bool Rememberme { get; set; }
    }
}