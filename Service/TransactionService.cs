using BusinessObject.Models;
using DataObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<Transaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _transactionRepository.GetTransactionsByUserIdAsync(userId);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.AddTransactionAsync(transaction);
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.UpdateTransactionAsync(transaction);
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.DeleteTransactionAsync(transaction);
        }

        public async Task AddListTransactionsAsync(IEnumerable<Transaction> transactions)
        {
            await _transactionRepository.AddListTransactionsAsync(transactions);
        }

        public async Task<decimal> GetMonthlySpendingByUserIdAsync(int userId)
        {
            return await _transactionRepository.GetMonthlySpendingByUserIdAsync(userId);
        }

        public async Task<decimal> GetTodaySpendingByUserIdAsync(int userId)
        {
            return await _transactionRepository.GetTodaySpendingByUserIdAsync(userId);
        }
        public async Task<decimal> GetMonthlySavingByUserIdAsync(int userId)
        {
            return await _transactionRepository.GetMonthlySavingByUserIdAsync(userId);
        }
        public async Task<Transaction> GetTransactionAsyncByTransactionId(int transactionId)
        {
            return await _transactionRepository.GetTransactionAsyncByTransactionId(transactionId);
        }
        public async Task<bool> UpdateTransactionCategoryAsync(int transactionId, int categoryId)
        {
            var transaction = await _transactionRepository.GetTransactionAsyncByTransactionId(transactionId);
            if (transaction == null) return false;

            transaction.CategoryId = categoryId;
            await _transactionRepository.UpdateTransactionAsync(transaction);
            return true;
        }

        public async Task<decimal> GetTotalSavedByUserInRangeAsync(int userId, DateOnly start, DateOnly end)
        {
            return await _transactionRepository.GetTotalSavedByUserInRangeAsync(userId, start, end);
        }
    }
}
