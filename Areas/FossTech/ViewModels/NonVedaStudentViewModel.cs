namespace FossTech.Areas.FossTech.ViewModels
{
    public class NonVedaStudentViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? BoardId { get; set; }
        public int? BranchId { get; set; }
        public int? StandardId { get; set; }
        public bool IsStudent { get; set; }
    }
}
