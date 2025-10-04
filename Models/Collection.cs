using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Fingers10.ExcelExport.Attributes;
using FossTech.Models.ProductModels;

namespace FossTech.Models
{
    public class Collection : ITimeStampedModel
    {
        [IncludeInReport(Order = 1)]
        public int Id { get; set; }

        [Required]
        [IncludeInReport(Order = 2)]
        public string Slug { get; set; }

        [Required]
        [IncludeInReport(Order = 3)]
        public string Name { get; set; }

        [IncludeInReport(Order = 4)]
        public string Image { get; set; }

        [IncludeInReport(Order = 5)]
        public virtual ICollection<CollectionProduct> Products { get; set; }

        [IncludeInReport(Order = 6)]
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
