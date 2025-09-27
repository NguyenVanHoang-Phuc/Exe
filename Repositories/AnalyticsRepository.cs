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

            // Tổng chi tiêu âm
            var totalExpense = transactions.Where(t => t.Amount < 0).Sum(t => -t.Amount);
            var totalIncome = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var totalBalance = totalIncome - totalExpense;

            var daysPassed = (DateTime.Now - start).Days + 1;
            var avgPerDay = daysPassed > 0 ? totalExpense / daysPassed : 0;

            // Top category
            var topCat = transactions
                .Where(t => t.Amount < 0 && t.Category != null)
                .GroupBy(t => t.Category!.Name)   // Lấy Name của Category
                .Select(g => new CategoryDto
                {
                    Name = g.Key,
                    Amount = -g.Sum(t => t.Amount)
                })
                .OrderByDescending(c => c.Amount)
                .FirstOrDefault();

            // Top 5 category
            var top5 = transactions
                .Where(t => t.Amount < 0 && t.Category != null)
                .GroupBy(t => t.Category!.Name)
                .Select(g => new CategoryAmountDto
                {
                    Name = g.Key,
                    Amount = -g.Sum(t => t.Amount),
                    Color = null
                })
                .OrderByDescending(c => c.Amount)
                .Take(5)
                .ToList();

            // Chi tiêu theo ngày
            var daily = transactions
                .Where(t => t.Amount < 0)
                .GroupBy(t => t.TransactionDate.Day)
                .Select(g => new DailyAmountDto
                {
                    Day = g.Key,
                    Amount = -g.Sum(t => t.Amount)
                })
                .OrderBy(d => d.Day)
                .ToList();

            // Tính BadCount
            var badTransactions = transactions.Where(t => t.TransactionType == "Bad").ToList();
            var badCount = badTransactions.Count;

            // BadRate tính theo % so với tổng chi tiêu
            var badRate = totalExpense > 0 ? badTransactions.Sum(t => -t.Amount) / totalExpense * 100 : 0;

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
                SmartTip = totalBalance < 0 ? "Chi tiêu vượt ngân sách" : "Tiết kiệm tốt"
            };
        }
    }
}
