using Fingers10.ExcelExport.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class ContactMessage : ITimeStampedModel
    {
        public int Id { get; set; }
        [Required]
        [IncludeInReport(Order = 1)]
        public string Name { get; set; }
        [EmailAddress]
        [Required]
        [IncludeInReport(Order = 2)]
        public string Email { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        [IncludeInReport(Order = 3)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [IncludeInReport(Order = 4)]
        public string Company { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "The message must be at least 5 characters long.")]
        [MaxLength(250, ErrorMessage = "The message must be max 250 characters long.")]
        [IncludeInReport(Order = 5)]
        public string Message { get; set; }

        [IncludeInReport(Order = 6)]
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}