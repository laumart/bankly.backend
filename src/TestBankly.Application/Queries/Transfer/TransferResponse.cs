using Newtonsoft.Json;
using TestBankly.Application.Base;

namespace TestBankly.Application.Queries.Transfer
{
    public class TransferResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TransactionId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Errors Errors { get; set; }
    }
}