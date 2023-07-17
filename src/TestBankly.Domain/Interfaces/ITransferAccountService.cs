using System.Threading.Tasks;
using TestBankly.Domain.Dto;

namespace TestBankly.Domain.Interfaces
{
    public interface ITransferAccountService
    {
        Task<AccountResponseDto> GetAccountByAccountNumberAsync(string accountNumer);
        Task<AccountTransactionResponseDto> PostAccountTransactionAsync(AccountTransactionRequestDto request);

    }
}
