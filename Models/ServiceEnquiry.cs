using FossTech.Models.ProductModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class ServiceEnquiry : ITimeStampedModel
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        public string Message { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
