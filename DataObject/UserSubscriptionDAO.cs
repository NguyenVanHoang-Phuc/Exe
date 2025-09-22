using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class UserSubscriptionDAO
    {
        private readonly FinanceAppDbContext _context;

        public UserSubscriptionDAO(FinanceAppDbContext context)
        {
            _context = context;
        }
        //hàm ni để ktra có phải là user có đăng ký gói premium k
        public Task<bool> HasActiveAsync(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return _context.UserSubscriptions.AsNoTracking()
                .AnyAsync(s =>
                    s.UserId == userId &&
                    s.Status == "active" &&
                    s.StartDate <= today &&
                    (s.EndDate == null || s.EndDate >= today));
        }

        public async Task UpsertActivateAsync(int userId, int planId, int months)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var current = await _context.UserSubscriptions
                .Where(s => s.UserId == userId && s.Status == "active" &&
                            s.StartDate <= today && (s.EndDate == null || s.EndDate >= today))
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (current is null)
            {
                _context.UserSubscriptions.Add(new UserSubscription
                {
                    UserId = userId,
                    PlanId = planId,
                    Status = "active",
                    StartDate = today,
                    EndDate = today.AddMonths(months)
                });
            }
            else
            {
                // Gia hạn: cộng thêm tháng vào EndDate hiện có
                var baseDate = current.EndDate is null || current.EndDate < today ? today : current.EndDate.Value;
                current.EndDate = baseDate.AddMonths(months);
                _context.UserSubscriptions.Update(current);
            }

            await _context.SaveChangesAsync();
        }
    }
}
