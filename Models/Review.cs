using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Fingers10.ExcelExport.Attributes;
using FossTech.Models.ProductModels;

namespace FossTech.Models
{
    public class Review : ITimeStampedModel
    {
        public int Id { get; set; }
        [DisplayName("Product")]
        [IncludeInReport(Order = 1)]
        public int ProductId { get; set; }

        [IncludeInReport(Order = 2)]
        public Product Product { get; set; }

        [DisplayName("User")]
        [IncludeInReport(Order = 3)]
        public string UserId { get; set; }

        [IncludeInReport(Order = 4)]
        public ApplicationUser User { get; set; }
        
        [Required]
        [IncludeInReport(Order = 5)]
        public int Rating { get; set; }
        [Required]
        [IncludeInReport(Order = 6)]
        public string Headline { get; set; }
        [Required]

        [IncludeInReport(Order = 7)]
        [DisplayName("Add a written review")]
        public string WrittenReview { get; set; }

        [IncludeInReport(Order = 8)]
        [DataType(DataType.DateTime)]

     
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
