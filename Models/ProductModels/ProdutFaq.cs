using System.ComponentModel;

namespace FossTech.Models.ProductModels
{
    public class ProdutFaq
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }
    }
}
