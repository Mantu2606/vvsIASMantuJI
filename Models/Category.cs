using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fingers10.ExcelExport.Attributes;
using FossTech.Models.ProductModels;

namespace FossTech.Models
{
    public class Category : ITimeStampedModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [IncludeInReport(Order = 1)]
        public string Name { get; set; }
        public string Slug { get; set; }

        [IncludeInReport(Order = 2)]
        public string Image { get; set; }
        public int SortOrder { get; set; }

        [IncludeInReport(Order = 3)]
        public bool Featured { get; set; }

        [IncludeInReport(Order = 4)]
        public bool Status { get; set; }

        [IncludeInReport(Order = 5)]
        public ICollection<SubCategory> SubCategories { get; set; }

        [IncludeInReport(Order = 6)]
        public virtual ICollection<Product> Products { get; set; }

        [IncludeInReport(Order = 7)]
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }


    }
}
