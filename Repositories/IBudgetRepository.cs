using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IBudgetRepository
    {
        Task AddRangeAsync(List<Budget> budgets);
        Task<List<Budget>> GetByGoalIdAsync(int goalId);
        Task DeleteRangeAsync(List<Budget> budgets);
        Task<Budget?> GetBudgetByUserIdAsync(int userId);
    }
}
