namespace FossTech.Configurations
{
    public interface IPhonePeConfiguration
    {
        string MerchantId { get; set; }
        string BaseURL { get; set; }
        string SaltKey { get; set; }
        string SaltIndex { get; set; }
    }
    public class PhonePeConfiguration: IPhonePeConfiguration
    {
        public string MerchantId { get; set; }
        public string BaseURL { get; set; }
        public string SaltKey { get; set; }
        public string SaltIndex { get; set; }

    }
}
