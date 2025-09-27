using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly GoalDAO _goalDAO;

        public GoalRepository(GoalDAO goalDAO)
        {
            _goalDAO = goalDAO;
        }

        public async Task<Goal> AddAsync(Goal goal)
        {
            return await _goalDAO.AddAsync(goal);
        }
        public async Task<Goal?> GetByIdAsync(int id)
        {
            return await _goalDAO.GetByIdAsync(id);
        }
        public async Task<List<Goal>> GetByUserIdAsync(int userId)
        {
            return await _goalDAO.GetByUserIdAsync(userId);
        }
        public async Task DeleteAsync(Goal goal)
        {
            await _goalDAO.DeleteAsync(goal);
        }
        public async Task<Goal?> GetActiveGoalByUserIdAsync(int userId)
        {
            return await _goalDAO.GetActiveGoalByUserIdAsync(userId);
        }
    }
}
