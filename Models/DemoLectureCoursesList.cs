namespace FossTech.Models
{
    public class DemoLectureCoursesList
    {
        public int Id { get; set; }
        public int DemoLecturesEnquiryId { get; set; }
        public DemoLecturesEnquiry DemoLecturesEnquiry { get; set; }
        public int ProductId { get; set; }
        public ProductModels.Product Product { get; set; }
    }
}
