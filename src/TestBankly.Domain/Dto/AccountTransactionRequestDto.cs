using TestBankly.Domain.Enums;

namespace TestBankly.Domain.Dto
{
    public class AccountTransactionRequestDto
    {
        public string AccountNumber { get; set; }
        public float Value { get; set; }
        public TransferType Type { get; set; }
    }

}
