namespace FossTech.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Img { get; set; }
        public int Order { get; set; }

        public int? SectionId { get; set; }
        public Section Section { get; set; }
    }
}