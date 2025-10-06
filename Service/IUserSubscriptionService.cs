using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IUserSubscriptionService
    {
        Task<UserSubscription> GetActiveSubscriptionAsync(int userId);
        Task<bool> HasActiveAsync(int userId);
        Task UpsertActivateAsync(int userId, int months, decimal amount);
    }
}
