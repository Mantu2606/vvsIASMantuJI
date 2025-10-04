using System.ComponentModel.DataAnnotations;

namespace FossTech.ViewModels
{
    public class ForgotPassword
    {
        [Required]
        public string Email { get; set; }
    }
}