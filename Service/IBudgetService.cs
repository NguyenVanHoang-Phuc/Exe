using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IBudgetService
    {
        Task<Budget?> GetBudgetByUserIdAsync(int userId);
        Task AddRangeAsync(List<Budget> budgets);
        Task DeleteRangeAsync(List<Budget> budgets);
        Task<Budget?> GetBudgetByUserMonthYearAsync(int userId, int month, int year);
        Task AddBudgetsForMonthsAsync(int userId, decimal amountLimit, int startMonth, int endMonth, int year);
    }
}
