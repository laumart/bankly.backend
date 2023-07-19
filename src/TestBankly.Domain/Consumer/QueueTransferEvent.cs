namespace TestBankly.Domain
{
    public class QueueTransferEvent
    {
        public string AccountOrigin { get; set; }
        public string AccountDestination { get; set; }
        public int Value { get; set; }
        public string TransactionId { get; set; }
    }
}
