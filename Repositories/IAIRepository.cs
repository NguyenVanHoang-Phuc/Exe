using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IAIRepository
    {
        Task<int> SaveRequestAsync(int userId, string prompt);
        Task<int> SaveAdviceAsync(int requestId, int userId, string advice, int? transactionId = null);
    }
}
