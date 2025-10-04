using System.ComponentModel.DataAnnotations;
using FossTech.Models;

namespace FossTech.ViewModels
{
    public class Register
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public BusinessProfile BusinessProfile { get; set; }
        [Required]
        [Display(Name = "Board")]
        public int BoardId { get; set; }

        [Required]
        [Display(Name = "Standard")]
        public int StandardId { get; set; }
        public int BranchId { get; set; }
        public Logo Logo { get; set; }
        public string MobileNumber { get; set; }
        public string Honeypot { get; set; }
    }
}