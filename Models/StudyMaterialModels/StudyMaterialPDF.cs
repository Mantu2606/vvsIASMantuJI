

namespace FossTech.Models.StudyMaterialModels
{
    public class StudyMaterialPDF
    {
        public int Id { get; set; }
        public int StudyMaterialId { get; set; }
        public StudyMaterial StudyMaterial { get; set; }
        public string PDF { get; set; }
        public int? SortOrder { get; set; }  
    }
}
