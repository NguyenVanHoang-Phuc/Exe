using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class GoalDAO
    {
        private readonly FinanceAppDbContext _context;

        public GoalDAO(FinanceAppDbContext context)
        {
            _context = context;
        }

        public async Task<Goal> AddAsync(Goal goal)
        {
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            return goal;
        }

        public async Task<Goal?> GetByIdAsync(int id)
        {
            return await _context.Goals
                .Include(g => g.Budgets)
                .FirstOrDefaultAsync(g => g.GoalId == id);
        }

        public async Task<List<Goal>> GetByUserIdAsync(int userId)
        {
            return await _context.Goals
                .Where(g => g.UserId == userId)
                .Include(g => g.Budgets)
                .ToListAsync();
        }

        public async Task DeleteAsync(Goal goal)
        {
            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
        }

        public async Task<Goal?> GetActiveGoalByUserIdAsync(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return await _context.Goals
                .Where(g => g.UserId == userId
                            && g.Status == "open"
                            && g.EndDate >= today)  // Chỉ kiểm tra chưa kết thúc
                .OrderBy(g => g.EndDate)     // Sắp xếp theo EndDate gần nhất
                .FirstOrDefaultAsync();
        }

    }
}
