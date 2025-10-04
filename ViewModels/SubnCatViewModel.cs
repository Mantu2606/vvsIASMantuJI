using System.Collections.Generic;
using FossTech.Models;

namespace FossTech.ViewModels
{
    public  class SubnCatViewModel
    { 
        public IEnumerable<Category> CategoryList { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategoryList { get; set; }
        public string StatusMessage { get; set; }
    }
}
