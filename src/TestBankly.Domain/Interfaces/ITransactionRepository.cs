using System;
using System.Threading.Tasks;
using TestBankly.Domain.Models;

namespace TestBankly.Domain.Interfaces
{
    public  interface ITransactionRepository
    {
        Task<Transaction> GetByTransactionId(string id);
        void Add(Transaction transaction);
        void Update(Transaction transaction);
    }
}
