using System.ComponentModel.DataAnnotations;

namespace WithoutIdentity.Models.ManagerViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }
        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Numero  do telefone")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}