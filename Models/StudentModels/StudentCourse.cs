namespace FossTech.Models.StudentModels
{
    public class StudentCourse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ProductId { get; set; }
        public ProductModels.Product Product { get; set; }

        public int BoardId { get; set; }
        public StudyMaterialModels.Board Board { get; set; }

        public int StandardId { get; set; }
        public StudyMaterialModels.Standard Standard { get; set; }

    }
}
