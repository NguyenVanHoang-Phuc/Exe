using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionDAO _transactionDAO;

        public TransactionRepository(TransactionDAO transactionDAO)
        {
            _transactionDAO = transactionDAO;
        }

        public async Task<List<Transaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _transactionDAO.GetTransactionsByUserIdAsync(userId);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _transactionDAO.AddTransactionAsync(transaction);
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            await _transactionDAO.UpdateTransactionAsync(transaction);
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            await _transactionDAO.DeleteTransactionAsync(transaction);
        }

        public async Task AddListTransactionsAsync(IEnumerable<Transaction> transactions)
        {
            await _transactionDAO.AddListTransactionsAsync(transactions);
        }
    }
}
