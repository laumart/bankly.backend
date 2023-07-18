using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TestBankly.Application.Queries.GetTransfer;
using TestBankly.Application.Queries.Transfer;

namespace TestBankly.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/fund-transfer")]
    public class FundTransferController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public FundTransferController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TransferResponse>> PostAsync([FromHeader(Name = "idempotency-Key")][Required] Guid idempotencyKey, 
            [FromBody] TransferRequest request, CancellationToken cancellation)
        {
            
            request.SetIdempotencyKey(idempotencyKey.ToString()); 
            var response = await _mediator.Send(request, cancellation);
            if (response != null && response.Errors != null)
                return Created("", response);
            else
                return UnprocessableEntity(response);

        }

        [HttpGet("{transactionId}")]
        public async Task<ActionResult<GetStatusResponse>> GetStatusAsync([Required] string transactionId, CancellationToken cancellation)
        {
            var response = await _mediator.Send(new GetStatusRequest { TransactionId = transactionId }, cancellation);
            return Ok(response);
        }
    }
}
