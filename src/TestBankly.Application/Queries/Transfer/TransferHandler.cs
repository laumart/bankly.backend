using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestBankly.Application.Base;
using TestBankly.Domain;
using TestBankly.Domain.Enums;
using TestBankly.Domain.Interfaces;

namespace TestBankly.Application.Queries.Transfer
{
    public class TransferHandler : IRequestHandler<TransferRequest, TransferResponse>
    {
        private readonly ILogger<TransferHandler> _logger;
        private readonly ITransferAccountService _transferAccountService;
        private readonly ITransactionRepository _repository;
        private readonly IPublishEndpoint _publisher;
        private readonly IBus _bus;

        public TransferHandler(ILogger<TransferHandler> logger,
            ITransferAccountService transferAccountService,
            ITransactionRepository repository,
            IPublishEndpoint publisher,
            IBus bus)
        {
            _logger = logger;
            _transferAccountService = transferAccountService;
            _repository = repository;
            _publisher = publisher;
            _bus = bus;
        }
        public async Task<TransferResponse> Handle(TransferRequest request, CancellationToken cancellationToken)
        {
            var response = await PreValidation(request);
            if (response != null) { return response; }

            var transactionModel = await _repository.GetByTransactionId(request.TransactionId);
            if (transactionModel == null)
            {
                transactionModel = InsertTransaction(request, StausTransactionType.Processing);
            }
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
                    var reverseTransfOrig = await _transferAccountService.PostAccountTransactionAsync(new Domain.Dto.AccountTransactionRequestDto
                    {
                        AccountNumber = request.AccountOrigin,
                        Value = request.Value,
                        Type = TransferType.Credit
                    });
                    transactionModel.Status = StausTransactionType.Error;
                    transactionModel.ErrorReason = responseTransDest.Errors.Message;
                }
            }
            else
            {
                transactionModel.Status = StausTransactionType.Error;
                transactionModel.ErrorReason = responseTransOrig.Errors.Message;
            }
            _repository.Update(transactionModel);

            return new TransferResponse
            {
                TransactionId = transactionModel.TransactionId,
                Errors = transactionModel.ErrorReason == null ? null : new Errors { Message = transactionModel.ErrorReason }
            };
        }

        private Domain.Models.Transaction InsertTransaction(TransferRequest request, StausTransactionType status)
        {
            var transactionModel = new Domain.Models.Transaction
            {
                AccountOrigin = request.AccountOrigin,
                AccountDestination = request.AccountDestination,
                TransactionId = request.TransactionId,
                Value = request.Value,
                Status = status
            };

            try
            {
                _repository.Add(transactionModel);
                return transactionModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao inserir transacao no DB {ex.Message} - {ex.StackTrace}");
                throw;
            }

        }

        private async Task<TransferResponse> PreValidation(TransferRequest request)
        {
            var transaction = await _repository.GetByTransactionId(request.TransactionId);
            _logger.LogInformation("inicio transfer handler");

            if (transaction != null && transaction.Status != StausTransactionType.InQueue)
                return new TransferResponse { Errors = new Errors { Message = "Essa chave de idempotencia está sendo utilizada" } };

            if (request.AccountOrigin == request.AccountDestination)
                return new TransferResponse { Errors = new Errors { Message = "Contas Origem e destino não podem ser iguais" } };

            if (request.Value == 0)
                return new TransferResponse { Errors = new Errors { Message = "Transferencia deve ter valor maior que zero" } };

            var responseOrig = await _transferAccountService.GetAccountByAccountNumberAsync(request.AccountOrigin);
            if (responseOrig.StatusCode == 404)
            {
                return new TransferResponse { Errors = new Errors { Message = "Conta Origem não encontrada" } };
            }
            else if (responseOrig.Balance < request.Value && responseOrig.Errors != null)
            {
                return new TransferResponse { Errors = new Errors { Message = "Saldo insuficiente para realizar a transação." } };
            }

            var responseDest = await _transferAccountService.GetAccountByAccountNumberAsync(request.AccountDestination);
            if (responseDest.StatusCode == 404)
            {
                return new TransferResponse { Errors = new Errors { Message = "Conta Destino não encontrada" } };
            }

            if (responseOrig?.StatusCode == 500 || responseDest?.StatusCode == 500)
            {
                if (transaction == null)
                {
                    transaction = InsertTransaction(request, StausTransactionType.InQueue);
                }
                await _publisher.Publish<QueueTransferEvent>(
                    new QueueTransferEvent
                    {
                        AccountDestination = request.AccountDestination,
                        AccountOrigin = request.AccountOrigin,
                        TransactionId = request.TransactionId,
                        Value = request.Value
                    }
                    );
                return new TransferResponse { Errors = new Errors { Message = "Erro na api de Accounts. A requisição será enviada para uma fila para processamento posterior" } };
            }

            return null;
        }
    }
}
