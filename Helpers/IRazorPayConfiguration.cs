namespace FossTech.Helpers
{
    public interface IRazorPayConfiguration
    {
        string KeyID { get; set; }
        string KeySecret { get; set; }
    }

    public class RazorPayConfiguration : IRazorPayConfiguration
    {
        public string KeyID { get; set; }
        public string KeySecret { get; set; }
    }
}
