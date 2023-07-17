using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TestBankly.Domain.Interfaces;

namespace TestBankly.Application.Queries.GetTransfer
{
    public class GetStatusHandler : IRequestHandler<GetStatusRequest, GetStatusResponse>
    {
        private readonly ILogger<GetStatusHandler> _logger;
        private readonly ITransactionRepository _repository;
        public GetStatusHandler(ILogger<GetStatusHandler> logger, ITransactionRepository repository)
        {
            _logger = logger;
            _repository = repository;
        } 
        public async Task<GetStatusResponse> Handle(GetStatusRequest request, CancellationToken cancellationToken)
        {
            var response = await _repository.GetByTransactionId(request.TransactionId);
            //if (response == null)
            return new GetStatusResponse { Status = response.Status, Message = response.ErrorReason };
        }
    }
}
