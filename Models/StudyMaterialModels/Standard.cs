namespace FossTech.Models.StudyMaterialModels
{
    public class Standard:ITimeStampedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? BoardId { get; set; }
        public Board Board { get; set; }
        public ICollection<Subject> Subjects { get; set; }
    }
}
