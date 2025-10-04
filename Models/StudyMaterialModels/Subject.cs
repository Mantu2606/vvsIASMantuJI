namespace FossTech.Models.StudyMaterialModels
{
    public class Subject : ITimeStampedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? StandardId { get; set; }
        public Standard Standard { get; set; }
        public ICollection<Chapter> Chapters { get; set; }

    }
}
