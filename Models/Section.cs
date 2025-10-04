using FossTech.Models.ProductModels;

namespace FossTech.Models
{
    public class Section: ITimeStampedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Image { get; set; }

        public string Slug { get; set; }
       
        public int SortOrder{  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public virtual List<Product> Products { get; set; }
         public ICollection<Image> Images { get; set; } 
    }
}
