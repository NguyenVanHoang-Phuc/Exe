using BusinessObject.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }
        public async Task<Budget?> GetBudgetByUserIdAsync(int userId)
        {
            return await _budgetRepository.GetBudgetByUserIdAsync(userId);
        }

        public async Task AddRangeAsync(List<Budget> budgets)
        {
            await _budgetRepository.AddRangeAsync(budgets);
        }
        public async Task DeleteRangeAsync(List<Budget> budgets)
        {
            await _budgetRepository.DeleteRangeAsync(budgets);
        }
        public async Task<Budget?> GetBudgetByUserMonthYearAsync(int userId, int month, int year)
        {
            return await _budgetRepository.GetBudgetByUserMonthYearAsync(userId, month, year);
        }

        public async Task AddBudgetsForMonthsAsync(int userId, decimal amountLimit, int startMonth, int endMonth, int year)
        {
            var today = DateTime.Today;
            var currentMonth = today.Month;
            var currentYear = today.Year;

            var budgets = new List<Budget>();

            for (int month = startMonth; month <= endMonth; month++)
            {
                // ❌ Không cho thêm budget quá khứ
                if (year < currentYear || (year == currentYear && month < currentMonth))
                    continue;

                // ❌ Nếu đã tồn tại budget cho tháng này thì bỏ qua
                var existing = await _budgetRepository.GetBudgetByUserMonthYearAsync(userId, month, year);
                if (existing != null) continue;

                budgets.Add(new Budget
                {
                    UserId = userId,
                    Month = month,
                    Year = year,
                    AmountLimit = amountLimit,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (budgets.Any())
            {
                await _budgetRepository.AddRangeAsync(budgets);
            }
        }
    }
}
