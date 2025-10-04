using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Models;
using FossTech.Models.ProductModels;

namespace FossTech.Models
{
    public class CollectionProduct : ITimeStampedModel
    {
        public int Id { get; set; }

        public int CollectionId { get; set; }
        public Collection Collection { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int? SectionId { get; set; }
        public Section Section { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
