using Fingers10.ExcelExport.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models
{
    public class SellerApplication : ITimeStampedModel
    {
        public int Id { get; set; }

        [Required]
        [IncludeInReport(Order = 1)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [IncludeInReport(Order = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [IncludeInReport(Order = 3)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [IncludeInReport(Order = 4)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [IncludeInReport(Order = 5)]
        [Display(Name = "Document Type")]
        [Required]
        public string DocumentType { get; set; }

        [IncludeInReport(Order = 6)]
        [Display(Name = "Document Image")]
        public string DocumentImage { get; set; }

        [IncludeInReport(Order = 7)]
        public string Status { get; set; }

        [IncludeInReport(Order = 8)]
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
