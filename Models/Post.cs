using Fingers10.ExcelExport.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class Post : ITimeStampedModel
    {
        public int Id { get; set; }
        [Required]
        [IncludeInReport(Order = 1)]
        public string Title { get; set; }
        [IncludeInReport(Order = 2)]
        public string Slug { get; set; }
        [Display(Name = "Meta description")]
        [IncludeInReport(Order = 3)]
        public string MetaDescription { get; set; }
        [Display(Name = "Meta keywords")]
        [IncludeInReport(Order = 4)]
        public string MetaKeywords { get; set; }

        [IncludeInReport(Order = 5)]
        public string Image { get; set; }
        public string Schema { get; set; }
        public int SortOrder { get; set; }

        [Required]
        [IncludeInReport(Order = 6)]
        public string Content { get; set; }

        [IncludeInReport(Order = 7)]

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]

        public string ImgAlt { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? SectionId { get;  set; }

        public Section Section { get; set; }
    }
}