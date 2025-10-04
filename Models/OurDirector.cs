using System;

namespace FossTech.Models
{
    public class OurDirector : ITimeStampedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Designation  { get; set; }
        public string Education { get; set; }
        public string Department { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? SectionId { get;  set; }

        public Section Section { get; set; }
    }
}




