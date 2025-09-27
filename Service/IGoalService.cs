using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IGoalService
    {
        Task<Goal> CreateGoalAsync(Goal goal);
        Task<List<Goal>> GetGoalsByUserAsync(int userId);
        Task DeleteGoalAsync(int goalId);
        Task<(Goal goal, decimal savedAmount, decimal progressPercent)?> GetActiveGoalWithProgressAsync(int userId);
    }
}
