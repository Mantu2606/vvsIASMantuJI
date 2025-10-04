using System;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class Banner : ITimeStampedModel
    {
        public int Id { get; set; }
       
        public string Image { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? SectionId { get;  set; }

        public Section Section { get; set; }
    }
}
