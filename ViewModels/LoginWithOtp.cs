using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.ViewModels
{
    public class LoginWithOtp
    {
        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
