using TestBankly.Domain.Enums;

namespace TestBankly.Domain.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string AccountOrigin { get; set; }
        public string AccountDestination { get; set; }
        public decimal Value { get; set; }
        public StausTransactionType Status { get; set; }
        public string ErrorReason { get; set; }
    }
}
