using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class TransactionDAO
    {
        private readonly FinanceAppDbContext _context;

        public TransactionDAO(FinanceAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.Transactions
                                 .Include(t => t.Category)
                                 .Where(t => t.UserId == userId)
                                 .OrderByDescending(t => t.TransactionDate)
                                 .ToListAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task AddListTransactionsAsync(IEnumerable<Transaction> transactions) 
        {
            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetMonthlySpendingByUserIdAsync(int userId)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionDate >= startOfMonth &&
                            t.TransactionDate <= endOfMonth &&
                            t.Amount < 0) // chỉ chi tiêu
                .SumAsync(t => (decimal?)(-t.Amount)) ?? 0; // đổi thành số dương
        }

        public async Task<decimal> GetTodaySpendingByUserIdAsync(int userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionDate >= today &&
                            t.TransactionDate < tomorrow &&
                            t.Amount < 0) // chỉ chi tiêu
                .SumAsync(t => (decimal?)(-t.Amount)) ?? 0; // đổi thành số dương
        }

        public async Task<decimal> GetMonthlySavingByUserIdAsync(int userId)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var income = await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionDate >= startOfMonth &&
                            t.TransactionDate <= endOfMonth &&
                            t.Amount >= 0)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var expense = await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionDate >= startOfMonth &&
                            t.TransactionDate <= endOfMonth &&
                            t.Amount < 0)
                .SumAsync(t => (decimal?)(-t.Amount)) ?? 0;

            return income - expense;
        }

        public async Task<Transaction> GetTransactionAsyncByTransactionId(int transactionId)
        {
            return await _context.Transactions
                                 .Include(t => t.Category) // nếu muốn lấy cả Category
                                 .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<decimal> GetTotalSavedByUserInRangeAsync(int userId, DateOnly start, DateOnly end)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId
                         && DateOnly.FromDateTime(t.CreatedAt) >= start
                         && DateOnly.FromDateTime(t.CreatedAt) <= end)
                .SumAsync(t => (decimal?)t.Amount ?? 0);
        }

    }
}
