using System.Collections;
using System.Collections.Generic;
using FossTech.Models;
using FossTech.Models.Menu;
using FossTech.Models.ProductModels;

namespace FossTech.ViewModels
{
    public class NavViewModel
    {
        public List<Page> More { get; set; }
        public Logo Logo { get; set; }

        public BusinessProfile BusinessProfile { get; set; }

        public List<Category> Categories { get; set; }

        public List<Product> Products { get; set; }

        public List<Menu> Menus { get; set; }
        public AboutUs AboutUs { get; set; }
        public List<Page> Pages { get; set; }

        public List<AboutUs> AboutsList {get; set;}
        public List<Menu> ShowFooter { get; set; }
        public WebSetting WebSetting { get; set; }
        public List<Branch> Branches { get; set; }
        public List<KeyWord> KeyWords { get; set; }
        public List<Section> Sections { get; set; }
        
    }
}
