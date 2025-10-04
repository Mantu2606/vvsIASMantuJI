using System.ComponentModel.DataAnnotations;
using FossTech.Models;
using Microsoft.AspNetCore.Authentication;

namespace FossTech.ViewModels
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public BusinessProfile BusinessProfile { get; set; }
        public Logo Logo { get;  set; }
        public List<AuthenticationScheme> ExternalLogins { get; set; }
    }
}