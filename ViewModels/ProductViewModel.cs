using System.Collections.Generic;
using FossTech.Models;
using FossTech.Models.ProductModels;

namespace FossTech.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Item { get; set; }
        public IEnumerable<Category> Category { get; set; }
    }
}
