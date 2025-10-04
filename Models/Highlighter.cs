using System;
using System.ComponentModel;

namespace FossTech.Models
{
    public class Highlighter : ITimeStampedModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        [DisplayName("Show/Hide")]
        public Boolean IsHighlighted { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? SectionId { get;  set; }

        public Section Section { get; set; }
    }
}
