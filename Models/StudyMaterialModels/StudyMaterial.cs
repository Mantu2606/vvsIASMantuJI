using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FossTech.Models.ProductModels;

namespace FossTech.Models.StudyMaterialModels
{
    public class StudyMaterial : ITimeStampedModel
    {
        public int Id { get; set; } 
        public string Name { get; set; }  
        public string ThumbnailImage { get; set; }  
        public virtual List<StudyMaterialPDF> PDFs { get; set; }
        public int? SortOrder { get; set; }  
        public int? ProductId { get; set; }  
        public Product Product { get; set; }
        public int? BoardId { get; set; }
        public int? StandardId { get; set; }
        public int? SubjectId { get; set; }
        public int? ChapterId { get; set; }
        public Board Board { get; set; }
        public Standard Standard { get; set; }
        public Subject Subject { get; set; }
        public Chapter Chapter { get; set; }
        public bool IsPaid { get; set; }

        public double? BasePrice { get; set; }
        public double? DiscountAmount { get; set; }
        [DisplayName("Final Price")]
        public double? FinalPrice => BasePrice != null ? DiscountAmount == null ? BasePrice : Math.Round((double)(BasePrice - DiscountAmount), 2) : null;

        [DisplayName("Discount Percentage")]
        public double? DiscountPercentage => DiscountAmount == null ? null : BasePrice != null ? (double?)Math.Round((double)(DiscountAmount / BasePrice * 100), 2) : null;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

        [Display(Name = "Payment Id")]
        public string PaymentId { get; set; }
        [Display(Name = "Transaction Id")]
        public string TransactionId { get; set; }
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }
        public double Amount { get; set; }
    }
}
