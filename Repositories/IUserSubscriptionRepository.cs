using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUserSubscriptionRepository
    {
        Task<bool> HasActiveAsync(int userId);
        Task UpsertActivateAsync(int userId, int planId, int months);
    }
}
