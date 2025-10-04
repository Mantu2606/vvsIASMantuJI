namespace FossTech.Models.TestPaperModels
{
    public class TestPaperPDF
    {
        public int Id { get; set; }
        public int TestPaperId { get; set; }
        public TestPaper TestPaper { get; set; }
        public string PDF { get; set; }
        public int? SortOrder { get; set; }  
    }
}
