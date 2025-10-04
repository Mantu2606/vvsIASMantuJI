using System.Collections.Generic;
using FossTech.Models;
using FossTech.Models.ProductModels;

namespace FossTech.ViewModels
{
    public class ItemViewModel
    {
        public Product Item { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<SubCategory> SubCategory { get; set; }
    }
}
