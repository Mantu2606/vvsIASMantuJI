using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models.Menu
{
    public class MenuSubCategory
    {
        public int Id { get; set; }

        public Menu Menu { get; set; }
        public int MenuId { get; set; }

        public SubCategory SubCategory { get; set; }

        public int SubCategoryId { get; set; }
    }
}
