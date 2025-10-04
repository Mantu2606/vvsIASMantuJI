namespace FossTech.Models
{
    public class BotAttempt
    {
        public int Id { get;set; }
        public string IpAddress { get;set; }
        public string UserAgent { get;set; }
        public DateTime AttemptedAt { get; set; }
        public string Reason { get; set; }
    }
}
