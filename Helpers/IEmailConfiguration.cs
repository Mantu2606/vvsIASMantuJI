namespace FossTech.Helpers
{
    public interface IEmailConfiguration
    {
        string Name { get; set; }

        string SmtpServer { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        string OrderEmail { get; set; }
    }

    public class EmailConfiguration : IEmailConfiguration
    {
        public string Name { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string OrderEmail { get; set; }
    }
}