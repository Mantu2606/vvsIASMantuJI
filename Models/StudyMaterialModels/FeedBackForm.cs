namespace FossTech.Models.StudyMaterialModels
{
    public class FeedBackForm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserId { get; set; }  

        public ApplicationUser User { get; set; }

        public bool IsSolved { get; set; }
        public DateTime? SolvedDate { get; set; }
    }
}
