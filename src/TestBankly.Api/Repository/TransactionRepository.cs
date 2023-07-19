using System;
using System.Linq;
using System.Threading.Tasks;
using TestBankly.Api.Data;
using TestBankly.Domain.Interfaces;
using TestBankly.Domain.Models;

namespace TestBankly.Infraestructure.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionContext _context;

        public TransactionRepository(TransactionContext context)
        {
            _context = context;
        }
        public void Add(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public void Update(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            _context.SaveChanges();
        }

        public async Task<Transaction> GetByTransactionId(string transactionId)
        {
            var searchByTransactionId = _context.Transactions
                .Where(t => t.Id > 0 && t.TransactionId == transactionId)
                .ToList();

            return searchByTransactionId.FirstOrDefault();
        }
    }
}
