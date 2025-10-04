using Fingers10.ExcelExport.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models
{
    public class Testimonial : ITimeStampedModel
    {
        
        public int Id { get; set; }

        [IncludeInReport(Order = 1)]
        public string Image { get; set; }

        [Required]
        [IncludeInReport(Order = 2)]
        public string Name { get; set; }
        public string Slug { get; set; }

        [Required]
        [IncludeInReport(Order = 3)]
        public string Review { get; set; }

        [DisplayName("Sort Order")]

        [IncludeInReport(Order = 4)]
        public int Order { get; set; }

        [IncludeInReport(Order = 5)]
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

        public int? SectionId { get; set; }

        public Section Section { get; set; }

    }
}
