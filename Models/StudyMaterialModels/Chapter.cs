namespace FossTech.Models.StudyMaterialModels
{
    public class Chapter: ITimeStampedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

        public int? SubjectId {  get; set; }
        public Subject Subject { get; set; }
    }
}
