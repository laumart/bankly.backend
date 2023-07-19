using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBankly.Domain.Dto
{
    public class AccountTransactionResponseDto
    {
        public bool Success { get; set; }
        public ErrorsDto Errors { get; set; }

    }
}
