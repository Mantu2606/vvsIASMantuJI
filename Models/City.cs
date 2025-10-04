using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<State> States { get; set; }
    }
}
