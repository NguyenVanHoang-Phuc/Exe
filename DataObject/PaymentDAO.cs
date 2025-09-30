using BusinessObject.Models;
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
                // Thêm thanh toán vào cơ sở dữ liệu
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving payment: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddPaymentAsync(int userId, int planId, decimal amount, string status, string method)
        {
            var payment = new Payment
            {
                UserId = userId,
                PlanId = planId,
                Amount = amount,
                Currency = "VND", // Hoặc lấy từ thông tin người dùng
                PaymentDate = DateTime.UtcNow,
                Status = status, // "paid" hoặc "failed"
                Method = method, // Ví dụ: "credit_card", "paypal", v.v.
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); // Lưu vào cơ sở dữ liệu

            return payment.PaymentId; // Trả về ID của payment vừa tạo
        }
    }
}
