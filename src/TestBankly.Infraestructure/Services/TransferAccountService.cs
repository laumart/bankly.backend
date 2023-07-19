using System;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using TestBankly.Application.Base;
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
            if (response.IsSuccessStatusCode)
            {
                return await DesserializerObject<AccountResponseDto>(response);
            }
            else 
            {
                var responseAccount = new AccountResponseDto { Errors = new ErrorsDto { Message = await response.Content.ReadAsStringAsync() } };
                responseAccount.SetStatusCode(Convert.ToInt32(response.StatusCode));
                return responseAccount;
            }
        }

        public async Task<AccountTransactionResponseDto> PostAccountTransactionAsync(AccountTransactionRequestDto request)
        {
            var url = $"api/Account";
            var response = await _httpClient.PostAsync(url, GetContent(request));

            if (response.IsSuccessStatusCode)
                return new AccountTransactionResponseDto { Success = true };

            return new AccountTransactionResponseDto { Success = false, Errors = new ErrorsDto { Message = await response.Content.ReadAsStringAsync() } };
        }

    }
}
