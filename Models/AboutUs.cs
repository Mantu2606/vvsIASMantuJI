using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class AboutUs : ITimeStampedModel
    {
        public int AboutUsId { get; set; }

        [Required]
        public string Title  { get; set; }

        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public string Slug { get; set; }

        public string WhyUs { get; set; }
        public string Vision { get; set; }
        public string Mission { get; set; }


        public int? SectionId { get; set; }

        public Section Section { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

        public int SortOrder {get; set;}
    }
}
