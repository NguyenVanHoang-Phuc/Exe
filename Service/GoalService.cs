using BusinessObject.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository _goalRepo;
        private readonly ITransactionRepository _tranRepo;

        public GoalService(IGoalRepository goalRepo, ITransactionRepository transactionRepository)
        {
            _goalRepo = goalRepo;
            _tranRepo = transactionRepository;
        }

        public async Task<Goal> CreateGoalAsync(Goal goal)
        {
            goal.CreatedAt = DateTime.Now;
            goal.Status = "open";
            if (goal.StartDate == null)
            {
                goal.StartDate = DateOnly.FromDateTime(DateTime.Today);
            }

            var createdGoal = await _goalRepo.AddAsync(goal);
            return createdGoal;
        }

        public async Task<List<Goal>> GetGoalsByUserAsync(int userId)
        {
            return await _goalRepo.GetByUserIdAsync(userId);
        }

        public async Task DeleteGoalAsync(int goalId)
        {
            var goal = await _goalRepo.GetByIdAsync(goalId);
            if (goal == null) throw new Exception("Goal not found");

            await _goalRepo.DeleteAsync(goal);
        }

        public async Task<Goal?> GetActiveGoalByUserAsync(int userId)
        {
            return await _goalRepo.GetActiveGoalByUserIdAsync(userId);
        }

        public async Task<(Goal goal, decimal savedAmount, decimal progressPercent)?> GetActiveGoalWithProgressAsync(int userId)
        {
            var goal = await _goalRepo.GetActiveGoalByUserIdAsync(userId);
            if (goal == null) return null;

            var startDate = goal.StartDate;

            var endDate = DateOnly.FromDateTime(DateTime.Today);

            var savedAmount = await _tranRepo.GetTotalSavedByUserInRangeAsync(userId, startDate, endDate);

            decimal progressPercent = 0;
            if (goal.TargetAmount > 0)
            {
                progressPercent = (savedAmount / goal.TargetAmount) * 100;
                if (progressPercent > 100) progressPercent = 100; // giới hạn max 100%
            }

            return (goal, savedAmount, progressPercent);
        }
    }
}
