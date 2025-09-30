using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AIRepository : IAIRepository
    {
        private readonly FinanceAppDbContext _db;

        public AIRepository(FinanceAppDbContext db)
        {
            _db = db;
        }

        public async Task<int> SaveRequestAsync(int userId, string prompt)
        {
            var req = new AiRequest
            {
                UserId = userId,
                PromptText = prompt,
                CreatedAt = DateTime.UtcNow
            };

            _db.AiRequests.Add(req);
            await _db.SaveChangesAsync();
            return req.RequestId;
        }

        public async Task<int> SaveAdviceAsync(int requestId, int userId, string advice, int? transactionId = null)
        {
            var adv = new AiAdvice
            {
                RequestId = requestId,
                UserId = userId,
                TransactionId = transactionId,
                AdviceText = advice,
                CreatedAt = DateTime.UtcNow
            };

            _db.AiAdvices.Add(adv);
            await _db.SaveChangesAsync();
            return adv.AdviceId;
        }
    }

}
