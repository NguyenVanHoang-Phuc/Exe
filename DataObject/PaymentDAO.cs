using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class PaymentDAO
    {
        private readonly FinanceAppDbContext _context;

        public PaymentDAO(FinanceAppDbContext context)
        {
            _context = context;
        }
        public async Task SavePaymentAsync(Payment payment)
        {
            try
            {

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving payment: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddPaymentAsync(int userId, decimal amount, string status, string method)
        {
            var payment = new Payment
            {
                UserId = userId,
                Amount = amount,
                Status = status,
                Method = method,
                Currency = "VND", 
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var userSubscription = new UserSubscription
            {
                UserId = userId,
                PaymentId = payment.PaymentId, 
                Status = "Active",  
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)) 
            };

            _context.UserSubscriptions.Add(userSubscription);
            await _context.SaveChangesAsync();  

            return payment.PaymentId;
        }

        public async Task<List<int>> GetUserIdsWithSuccessfulPaymentsAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == "active")
                .Select(p => p.UserId)
                .Distinct()
                .ToListAsync();
        }
    }
}
