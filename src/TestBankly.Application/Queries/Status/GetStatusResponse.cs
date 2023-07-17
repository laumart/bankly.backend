using Newtonsoft.Json;
using TestBankly.Domain.Enums;

namespace TestBankly.Application.Queries.GetTransfer
{
    public class GetStatusResponse
    {
        public StausTransactionType Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}
