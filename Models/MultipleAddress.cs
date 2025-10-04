using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FossTech.Models
{
    public class MultipleAddress
    {
        public int Id { get; set; }
        [DisplayName("Address")]
        [Required]
        public string Address { get; set; }

        [DisplayName("City")]
        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [DisplayName("Country")]
        [Required]
        public string Country { get; set; }

        [Required]
        public string Pincode { get; set; }
        [DisplayName("Google My Business Link")]
        public string GoogleMyBusinessLink { get; set; }
        public int SortOrder { get; set; }
    }
}
