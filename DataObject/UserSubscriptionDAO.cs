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
        private readonly PaymentDAO _paymentDAO;

        public UserSubscriptionDAO(FinanceAppDbContext context, PaymentDAO paymentDAO)
        {
            _context = context;
            _paymentDAO = paymentDAO;
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

        public async Task UpsertActivateAsync(int userId, int planId, int months, decimal amount)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var current = await _context.UserSubscriptions
                .Where(s => s.UserId == userId && s.Status == "active" &&
                            s.StartDate <= today && (s.EndDate == null || s.EndDate >= today))
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            var paymentId = await _paymentDAO.AddPaymentAsync(userId, planId, amount, "paid", "credit_card");

            if (current is null)
            {
                _context.UserSubscriptions.Add(new UserSubscription
                {
                    UserId = userId,
                    PlanId = planId,
                    Status = "active",
                    StartDate = today,
                    EndDate = today.AddMonths(months),
                    SubscriptionId = paymentId
                });
            }
            else
            {
                var baseDate = current.EndDate is null || current.EndDate < today ? today : current.EndDate.Value;
                current.EndDate = baseDate.AddMonths(months);
                current.Status = "active";
                _context.UserSubscriptions.Update(current);
            }

            await _context.SaveChangesAsync();
        }
    }
}
