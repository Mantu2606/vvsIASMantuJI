using System;
using System.ComponentModel;

namespace FossTech.Models
{
    public class SliderImage : ITimeStampedModel
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }

        [DisplayName("Mobile Banner")]
        public string MobileImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public string Title { get; set; }
        public int? SectionId { get; set; }

        public Section Section { get; set; }
    }
}