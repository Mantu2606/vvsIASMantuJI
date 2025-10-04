using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models
{
    public class Team : ITimeStampedModel
    {
        public int Id { get; set; }

        public string Image { get; set; }

        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Experience { get; set; }

        [Required]
        public string Designation { get; set; }
        public string Description { get; set; }

        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? SectionId { get; set; }

        public Section Section { get; set; }
    }
}
