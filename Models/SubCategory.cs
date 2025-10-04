using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FossTech.Models.ProductModels;

namespace FossTech.Models
{
    public class SubCategory : ITimeStampedModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Sub Category")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

    }
}
