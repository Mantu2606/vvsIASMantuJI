using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Areas.FossTech.ViewModels
{
    public class Colors
    {
        [Required]
        [DisplayName("Primary Color")]
        public string PrimaryColor { get; set; }    
    }
}
