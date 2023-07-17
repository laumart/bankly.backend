using MediatR;

namespace TestBankly.Application.Queries.GetTransfer
{
    public class GetStatusRequest : IRequest<GetStatusResponse>
    {
        public string TransactionId { get; set; }
    }
}
