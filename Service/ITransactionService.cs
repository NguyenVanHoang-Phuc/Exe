using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsByUserIdAsync(int userId);
        Task AddTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(Transaction transaction);
        Task AddListTransactionsAsync(IEnumerable<Transaction> transactions);
        Task<decimal> GetMonthlySpendingByUserIdAsync(int userId);
        Task<decimal> GetTodaySpendingByUserIdAsync(int userId);
        Task<decimal> GetMonthlySavingByUserIdAsync(int userId);
        Task<Transaction> GetTransactionAsyncByTransactionId(int transactionId);
        Task<bool> UpdateTransactionCategoryAsync(int transactionId, int categoryId);
        Task<decimal> GetTotalSavedByUserInRangeAsync(int userId, DateOnly start, DateOnly end);
    }
}
