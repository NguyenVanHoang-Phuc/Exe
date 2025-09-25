using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ITransactionRepository
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
    }
}
