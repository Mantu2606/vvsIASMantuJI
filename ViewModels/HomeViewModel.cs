using System.Collections.Generic;
using DocumentFormat.OpenXml.Office.CustomUI;
using FossTech.Models;
using FossTech.Models.Menu;
using FossTech.Models.ProductModels;

namespace FossTech.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> _Category { get; set; }
        

        public List<SliderImage> SliderImages { get; set; }

        public AboutUs AboutUs { get; set; }

        public string Banner { get; set; }
        public List<Post> Updates { get; set; }
        public List<Image> _Image { get; set; }
        public List<Product> _items { get; set; }
        public List<Product> _bestProduct { get; set; }

        public Post _supdate { get; set; }
        public BusinessProfile BusinessProfile { get; set; }
        public Product _sitem { get; set; }
       

        public List<List<Category>> _ChunkCategory { get; set; }
        public List<List<Product>> _ChunkProduct { get; set; }
        public List<List<Product>> _ChunkBest { get; set; }
        public List<Testimonial> Testimonials { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<Team> Team { get; set; }
        public WebSetting WebSetting { get; set; }
        public List<Counter> Counters { get; set; }
        public List<OurDirector> OurDirector { get; set; }
        public List<OurHOD> OurHOD { get; set; }
        public Highlighter Highlighter { get; set; }
        public List<Update> NewUpdate { get; set; }
        public List<LatestNews> LatestNews { get; set; }
        public List<OurTopper> Toppers { get; set; }
        public List<Placement> Placement { get; set; }

        public List<Section>Sections { get; set; }  
        public RegistrationBanner RegistrationBanners { get; set; }

       
    }
}
