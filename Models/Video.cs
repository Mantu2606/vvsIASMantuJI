using Fingers10.ExcelExport.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models
{
    public class Video
    {
        public int Id { get; set; }
        [Required]
        [IncludeInReport(Order = 1)]
        public string Title { get; set; }

        [Required]
        [IncludeInReport(Order = 2)]
        public string Code { get; set; }
        public string Slug { get; set; }
        public int SortOrder { get; set; }

        public int? SectionId { get; set; }

        public Section Section { get; set; }
    }
}
