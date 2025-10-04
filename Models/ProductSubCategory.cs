using FossTech.Models.ProductModels;

namespace FossTech.Models
{
    public class ProductSubCategory
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
    }
}
