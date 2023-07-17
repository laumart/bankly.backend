using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestBankly.Application.Base;
using TestBankly.Domain.Enums;
using TestBankly.Domain.Interfaces;
using TestBankly.Domain.Models;

namespace TestBankly.Application.Queries.Transfer
{
    public class TransferHandler : IRequestHandler<TransferRequest, TransferResponse>
    {
        private readonly ILogger<TransferHandler> _logger;
        private readonly ITransferAccountService _transferAccountService;
        private readonly ITransactionRepository _repository;

        public TransferHandler(ILogger<TransferHandler> logger,
            ITransferAccountService transferAccountService,
            ITransactionRepository repository)
        {
            _logger = logger;
            _transferAccountService = transferAccountService;
            _repository = repository;
        }
        public async Task<TransferResponse> Handle(TransferRequest request, CancellationToken cancellationToken)
        {
            if (request.AccountOrigin == request.AccountDestination)
                return new TransferResponse { Errors = new Errors { Message = "Contas Origem e destino não podem ser iguais" } };

            if (request.Value == 0)
                return new TransferResponse { Errors = new Errors { Message = "Transferencia deve ter valor maior que zero" } };

            var responseOrig = await _transferAccountService.GetAccountByAccountNumberAsync(request.AccountOrigin);
            if (responseOrig == null)
            {
                return new TransferResponse { Errors = new Errors { Message = "Conta Origem não encontrada" } };
            }

            var responseDest = await _transferAccountService.GetAccountByAccountNumberAsync(request.AccountDestination);
            if (responseDest == null)
            {
                return new TransferResponse { Errors = new Errors { Message = "Conta Destino não encontrada" } };
            }

            var transactionModel = new Transaction
            {
                AccountOrigin = request.AccountOrigin,
                AccountDestination = request.AccountDestination,
                TransactionId = Guid.NewGuid().ToString(),
                Value = request.Value,
                Status = StausTransactionType.Processing
            };

            _repository.Add(transactionModel);

            var responseTransOrig = await _transferAccountService.PostAccountTransactionAsync(new Domain.Dto.AccountTransactionRequestDto
            {
                AccountNumber = request.AccountOrigin,
                Value = request.Value,
                Type = TransferType.Debit
            });

            if (responseTransOrig.Success)
            {
                var responseTransDest = await _transferAccountService.PostAccountTransactionAsync(new Domain.Dto.AccountTransactionRequestDto
                {
                    AccountNumber = request.AccountDestination,
                    Value = request.Value,
                    Type = TransferType.Credit
                });

                if (responseTransDest.Success)
                {
                    transactionModel.Status = StausTransactionType.Confirmed;
                }
                else
                {
                    transactionModel.Status = StausTransactionType.Error;
                    transactionModel.ErrorReason = responseTransDest.Errors.Message;
                }

                _repository.Update(transactionModel);
            }

            
            return new TransferResponse { TransactionId = transactionModel.TransactionId };
        }
    }
}
