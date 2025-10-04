namespace FossTech.Helpers
{
    public interface IMySmsShopConfiguration
    {
        string Username { get; }
        string Password { get; }
        string BaseURL { get; }
        string SenderId { get; }
    }

    public class MySmsShopConfiguration  : IMySmsShopConfiguration
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string BaseURL { get; set; }

        public string SenderId { get; set; }
    }
}
