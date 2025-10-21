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

        public async Task<Budget?> GetBudgetByUserMonthYearAsync(int userId, int month, int year)
        {
            return await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == month && b.Year == year);
        }


        public async Task UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Budget>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .ThenByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Budget> GetByIdAsync(int id)
        {
            return await _context.Budgets.FindAsync(id);
        }
    }
}
