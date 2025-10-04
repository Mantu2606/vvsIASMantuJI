using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class FAQ
    {
        public int Id { get; set; }
        [Required]
        public string Question { get; set; }
        public string Slug { get; set; }
        [Required]
        public string Answer { get; set; }

        public int Order { get; set; }
        public int? SectionId { get; set; }
        public Section Section { get; set; }
    }
}
