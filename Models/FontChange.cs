using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class FontChange
    {
        public int Id { get; set; }
        [DisplayName("Add Font Links")]
        public string AddFontLinks { get; set; }
        [DisplayName("Css Rules To Specify Font Family")]
        public string Css { get; set; }
    }
}
