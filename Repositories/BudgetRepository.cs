using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly BudgetDAO _budgetDAO;
        public BudgetRepository(BudgetDAO budgetDAO)
        {
            _budgetDAO = budgetDAO;
        }

        public async Task AddRangeAsync(List<Budget> budgets)
        {
            await _budgetDAO.AddRangeAsync(budgets);
        }
        public async Task DeleteRangeAsync(List<Budget> budgets)
        {
            await _budgetDAO.DeleteRangeAsync(budgets);
        }
        public async Task<Budget?> GetBudgetByUserIdAsync(int userId)
        {
            return await _budgetDAO.GetBudgetByUserIdAsync(userId);
        }

        public async Task<Budget?> GetBudgetByUserMonthYearAsync(int userId, int month, int year)
        {
            return await _budgetDAO.GetBudgetByUserMonthYearAsync(userId, month, year);
        }
    }
}
