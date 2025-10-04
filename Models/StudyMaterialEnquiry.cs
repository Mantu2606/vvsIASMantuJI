namespace FossTech.Models
{
    public class StudyMaterialEnquiry
    {
        public int Id { get; set; }  
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string MaterialName { get; set; }
        public string Message {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
