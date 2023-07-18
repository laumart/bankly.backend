using System;
using System.Threading.Tasks;
using TestBankly.Domain.Models;

namespace TestBankly.Domain.Interfaces
{
    public  interface ITransactionRepository
    {
        Task<Transaction> GetByTransactionId(string TransactionId);
        void Add(Transaction transaction);
        void Update(Transaction transaction);
    }
}
