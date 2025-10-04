using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class HeaderCode
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }
    }
}