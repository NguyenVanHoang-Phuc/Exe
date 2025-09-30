using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly TransactionDAO _transactionDao;

        public AnalyticsRepository(TransactionDAO transactionDao)
        {
            _transactionDao = transactionDao;
        }

        public async Task<AnalyticsDto> GetMonthlyAnalyticsAsync(int userId, DateTime start, DateTime end)
        {
            var transactions = await _transactionDao.GetTransactionsByUserInRangeAsync(userId, start, end);

            // Chỉ còn chi tiêu (luôn dương)
            var expenseTransactions = transactions.ToList();
            var totalExpense = expenseTransactions.Sum(t => t.Amount);

            var daysPassed = (DateTime.Now - start).Days + 1;
            var avgPerDay = daysPassed > 0 ? totalExpense / daysPassed : 0;

            // Top category
            var topCat = expenseTransactions
                .Where(t => t.Category != null)
                .GroupBy(t => t.Category!.Name)
                .Select(g => new CategoryDto
                {
                    Name = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(c => c.Amount)
                .FirstOrDefault();

            // Top 5 category
            var top5 = expenseTransactions
                .Where(t => t.Category != null)
                .GroupBy(t => t.Category!.Name)
                .Select(g => new CategoryAmountDto
                {
                    Name = g.Key,
                    Amount = g.Sum(t => t.Amount),
                    Color = null
                })
                .OrderByDescending(c => c.Amount)
                .Take(5)
                .ToList();

            // Chi tiêu theo ngày
            var daily = expenseTransactions
                .GroupBy(t => t.TransactionDate.Day)
                .Select(g => new DailyAmountDto
                {
                    Day = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderBy(d => d.Day)
                .ToList();

            // Tính BadCount
            var badTransactions = expenseTransactions.Where(t => t.TransactionType == "Bad").ToList();
            var badCount = badTransactions.Count;

            // BadRate tính theo % so với tổng chi tiêu
            var badRate = totalExpense > 0 ? badTransactions.Sum(t => t.Amount) / totalExpense * 100 : 0;

            return new AnalyticsDto
            {
                TotalMonth = totalExpense,
                AvgPerDay = avgPerDay,
                TopCategory = topCat,
                Categories = top5,
                Daily = daily,
                Top5 = top5,
                BadCount = badCount,
                BadRate = badRate,
                SmartTip = totalExpense > 0 ? "Theo dõi chi tiêu để không vượt quá ngân sách" : "Chưa có chi tiêu"
            };
        }
    }
}
