using FossTech.Models.ProductModels;
using System;

namespace FossTech.Models
{
    public class CoursesEnquiry : ITimeStampedModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
