namespace FossTech.Models.Menu
{
    public class MenuPage
    {
        public int Id { get; set; }

        public Menu Menu { get; set; }
        public int MenuId { get; set; }

        public Page Page { get; set; }

        public int PageId { get; set; }
    }
}
