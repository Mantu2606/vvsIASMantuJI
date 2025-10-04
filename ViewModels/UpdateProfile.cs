using System.ComponentModel.DataAnnotations;

namespace FossTech.ViewModels
{
    public class UpdateProfile
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set;  }
    }
}