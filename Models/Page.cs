using Fingers10.ExcelExport.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class Page : ITimeStampedModel
    {
        public int Id { get; set; }
        [Required]
        [IncludeInReport(Order = 1)]
        public string Name { get; set; }

        [Required]
        [IncludeInReport(Order = 2)]
        public string Slug { get; set; }

        [DisplayName("Banner Image")]
        [IncludeInReport(Order = 3)]
        public string BannerImage { get; set; }

        [IncludeInReport(Order = 4)]
        public int Order { get; set; }
        [Required]
        [IncludeInReport(Order = 5)]
        public string Content { get; set; }

        [IncludeInReport(Order = 6)]
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

        public string ImgAlt { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
    }
}
