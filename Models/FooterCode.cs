using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class FooterCode
    {
        public int Id { get; set; }

        
        [Required]
        public string Code { get; set; }

    }
}
