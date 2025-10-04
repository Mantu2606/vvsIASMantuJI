namespace FossTech.Models.Menu
{
    public class MenuCategory
    {
        public int Id { get; set; }

        public Menu Menu { get; set; }
        public int MenuId { get; set; }

        public Category Category { get; set; }

        public int CategoryId { get; set; }
    }
}
