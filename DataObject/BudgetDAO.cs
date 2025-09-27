using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class BudgetDAO
    {
        private readonly FinanceAppDbContext _context;

        public BudgetDAO(FinanceAppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<Budget> budgets)
        {
            _context.Budgets.AddRange(budgets);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Budget>> GetByGoalIdAsync(int goalId)
        {
            return await _context.Budgets
                .Where(b => b.GoalId == goalId)
                .OrderBy(b => b.Year).ThenBy(b => b.Month)
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(List<Budget> budgets)
        {
            _context.Budgets.RemoveRange(budgets);
            await _context.SaveChangesAsync();
        }

        public async Task<Budget?> GetBudgetByUserIdAsync(int userId)
        {
            var now = DateTime.Now;

            return await _context.Budgets
                .Where(b => b.UserId == userId
                            && b.Year == now.Year
                            && b.Month == now.Month)
                .FirstOrDefaultAsync();
        }

    }
}
