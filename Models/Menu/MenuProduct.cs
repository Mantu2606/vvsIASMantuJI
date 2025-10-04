using FossTech.Models.ProductModels;

namespace FossTech.Models.Menu
{
    public class MenuProduct
    {
        public int Id { get; set; }

        public Menu Menu { get; set; }
        public int MenuId { get; set; }

        public Product Product { get; set; }

        public int ProductId { get; set; }
    }
}
