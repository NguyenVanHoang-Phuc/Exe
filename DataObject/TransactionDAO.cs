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
                            t.TransactionDate <= endOfMonth
                            ) // chỉ chi tiêu
                .SumAsync(t => (decimal?)t.Amount) ?? 0; // đổi thành số dương
        }

        public async Task<decimal> GetTodaySpendingByUserIdAsync(int userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionDate >= today &&
                            t.TransactionDate < tomorrow 
                            ) // chỉ chi tiêu
                .SumAsync(t => (decimal?)t.Amount ?? 0); // đổi thành số dương
        }

        public async Task<decimal> GetMonthlySavingByUserIdAsync(int userId)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Lấy ngân sách tháng hiện tại
            var budget = await _context.Budgets
                .Where(b => b.UserId == userId && b.Month == now.Month && b.Year == now.Year)
                .Select(b => (decimal?)b.AmountLimit)
                .FirstOrDefaultAsync() ?? 0;

            // Tổng chi tiêu tháng hiện tại (chỉ tính các giao dịch < 0 hoặc transaction_type = expense)
            var spent = await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionDate >= startOfMonth &&
                            t.TransactionDate <= endOfMonth)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            // Vì Amount lưu chi tiêu thường âm, cần lấy trị tuyệt đối
            var expense = spent < 0 ? Math.Abs(spent) : spent;

            // Tiết kiệm/thực còn lại = budget – expense
            return budget - expense;
        }


        public async Task<Transaction> GetTransactionAsyncByTransactionId(int transactionId)
        {
            return await _context.Transactions
                                 .Include(t => t.Category) 
                                 .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<decimal> GetTotalSavedByUserInRangeAsync(int userId, DateOnly start, DateOnly end)
        {
            decimal totalSaving = 0;

            // duyệt từng tháng trong khoảng
            var current = new DateOnly(start.Year, start.Month, 1);
            var last = new DateOnly(end.Year, end.Month, 1);

            while (current <= last)
            {
                var month = current.Month;
                var year = current.Year;

                // lấy budget của tháng đó
                var budget = await _context.Budgets
                    .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
                    .Select(b => (decimal?)b.AmountLimit)
                    .FirstOrDefaultAsync() ?? 0;

                // lấy chi tiêu tháng đó
                var startOfMonth = new DateTime(year, month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var expense = await _context.Transactions
                    .Where(t => t.UserId == userId &&
                                t.TransactionDate >= startOfMonth &&
                                t.TransactionDate <= endOfMonth)
                    .SumAsync(t => (decimal?)t.Amount) ?? 0;

                // cộng dồn saving (có thể âm nếu chi vượt budget)
                totalSaving += (budget - expense);

                current = current.AddMonths(1);
            }

            return totalSaving;
        }


        public async Task<List<Transaction>> GetTransactionsByUserInRangeAsync(int userId, DateTime start, DateTime end)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.TransactionDate >= start && t.TransactionDate <= end)
                .ToListAsync();
        }
    }
}
