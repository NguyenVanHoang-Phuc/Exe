using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAO _paymentDAO;
        public PaymentRepository(PaymentDAO paymentDAO)
        {
            _paymentDAO = paymentDAO;
        }
        public async Task SavePaymentAsync(Payment payment)
        {
            await _paymentDAO.SavePaymentAsync(payment);
        }
        public async Task<int> AddPaymentAsync(int userId, int planId, decimal amount, string status, string method)
        {
            return await _paymentDAO.AddPaymentAsync(userId, planId, amount, status, method);
        }
    }
}
