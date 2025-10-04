using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class Enquiry
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
