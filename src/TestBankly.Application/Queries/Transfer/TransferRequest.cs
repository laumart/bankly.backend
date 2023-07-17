using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace TestBankly.Application.Queries.Transfer
{
    public class TransferRequest : IRequest<TransferResponse>
    {
        [Required]
        public string AccountOrigin { get; set; }
        [Required] 
        public string AccountDestination { get; set; }
        [Required]
        public int Value { get; set; }
    }

}
