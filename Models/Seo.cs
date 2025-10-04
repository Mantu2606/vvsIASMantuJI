using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class Seo
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Title")]
        public string MetaTitle { get; set; }
        [Required]
        [DisplayName("Meta Description")]
        public string MetaDiscription { get; set; }

        public string CanonicalTag { get; set; }
        public string OgTitle { get; set; }
        public string OgDescription { get; set; }
        public string  OgUrl { get; set; }
        public string OgSiteName { get; set; }
        public string MetaTwitter { get; set; }
        public string MetaTwitterDescription { get; set; }
        public string TwitterTitle { get; set; }
        [DisplayName("MS Validate")]
        public string MsValidate { get; set; }
        [DisplayName("Google SIte Verification")]

        public string GSiteVerivation { get; set; }

        public string MetaTags { get; set; }
    }
}
