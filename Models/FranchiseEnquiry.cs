using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace FossTech.Models
{
    public class FranchiseEnquiry : ITimeStampedModel
    {
        public int Id { get; set; }
        
        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [Required]
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Pincode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
