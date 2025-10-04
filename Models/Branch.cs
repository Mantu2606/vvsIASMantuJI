using System.ComponentModel;

namespace FossTech.Models
{
    public class Branch
    {
        public int Id { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string GoogleIframe { get; set; }
        public string GoogleMapLink { get; set; }
        public int SortOrder { get; set; }
        public int? SectionId { get; set; }
        public Section Section { get; set; }
    }
}
