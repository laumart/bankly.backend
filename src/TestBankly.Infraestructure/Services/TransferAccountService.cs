using System;
using System.Net.Http;
using System.Threading.Tasks;
using TestBankly.Domain.Dto;
using TestBankly.Domain.Interfaces;
using TestBankly.Infraestructure.Configurations;

namespace TestBankly.Infraestructure.Services
{
    public class TransferAccountService : ServiceBase, ITransferAccountService
    {
        private readonly HttpClient _httpClient;

        public TransferAccountService(HttpClient httpClient, ServiceSettings settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.UrlAccounts);
        }

        public async Task<AccountResponseDto> GetAccountByAccountNumberAsync(string accountNumer)
        {
            var url = $"api/Account/{accountNumer}";
            var response = await _httpClient.GetAsync(url);

            return await DesserializerObject<AccountResponseDto>(response);
        }

        public async Task<AccountTransactionResponseDto> PostAccountTransactionAsync(AccountTransactionRequestDto request)
        {
            var url = $"api/Account";
            var response = await _httpClient.PostAsync(url, GetContent(request));

            if (response.IsSuccessStatusCode)
                return new AccountTransactionResponseDto { Success = true };

            return new AccountTransactionResponseDto { Errors = new ErrorsDto { Message = await response.Content.ReadAsStringAsync() } };
        }

    }
}
