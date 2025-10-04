namespace FossTech.Models.StudyMaterialModels
{
    public class Board:ITimeStampedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<Standard> Standards { get; set; }

    }
}
