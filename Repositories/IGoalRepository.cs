using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IGoalRepository
    {
        Task<Goal> AddAsync(Goal goal);
        Task<Goal?> GetByIdAsync(int id);
        Task<List<Goal>> GetByUserIdAsync(int userId);
        Task DeleteAsync(Goal goal);
        Task<Goal?> GetActiveGoalByUserIdAsync(int userId);
    }
}
