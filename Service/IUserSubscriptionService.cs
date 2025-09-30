using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IUserSubscriptionService
    {
        Task<bool> HasActiveAsync(int userId);
        Task UpsertActivateAsync(int userId, int planId, int months, decimal amount);
    }
}
