namespace FossTech.Areas.Dashboard.ViewModels
{
    public class CheckoutOrderCompletedDto
    {
        public string Type { get; set; }
        public string Event { get; set; }
        public PayloadDto Payload { get; set; }
    }

    public class PayloadDto
    {
        public string MerchantId { get; set; }
        public string MerchantOrderId { get; set; }
        public string OrderId { get; set; }
        public string State { get; set; }
        public int Amount { get; set; }
        public int PayableAmount { get; set; }
        public int FeeAmount { get; set; }
        public long ExpireAt { get; set; }
        public Dictionary<string, object> MetaInfo { get; set; }
        public List<PaymentDetailDto> PaymentDetails { get; set; }
    }

    public class PaymentDetailDto
    {
        public string TransactionId { get; set; }
        public string PaymentMode { get; set; }
        public long Timestamp { get; set; }
        public int Amount { get; set; }
        public int PayableAmount { get; set; }
        public int FeeAmount { get; set; }
        public string State { get; set; }
        public InstrumentDto Instrument { get; set; }
        public RailDto Rail { get; set; }
        public List<SplitInstrumentDto> SplitInstruments { get; set; }
    }

    public class InstrumentDto
    {
        public string Type { get; set; }
        public string Ifsc { get; set; }
        public string AccountType { get; set; }
    }

    public class RailDto
    {
        public string Type { get; set; }
        public string Utr { get; set; }
        public string UpiTransactionId { get; set; }
    }

    public class SplitInstrumentDto
    {
        public InstrumentDto Instrument { get; set; }
        public RailDto Rail { get; set; }
        public int Amount { get; set; }
    }
}
