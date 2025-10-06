using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataObject.PayOSDAO;

namespace Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAO _paymentDAO;
        private readonly PayOSDAO _payOSDAO;

        public PaymentRepository(PaymentDAO paymentDAO, PayOSDAO payOSDAO)
        {
            _paymentDAO = paymentDAO;
            _payOSDAO = payOSDAO;
        }
        public async Task SavePaymentAsync(Payment payment)
        {
            await _paymentDAO.SavePaymentAsync(payment);
        }
        public async Task<int> AddPaymentAsync(int userId, decimal amount, string status, string method) => await _paymentDAO.AddPaymentAsync(userId, amount, status, method);
        public async Task<PaymentResult> GetPaymentStatusAsync(string transactionId) => await _payOSDAO.GetPaymentStatusAsync(transactionId);
        public async Task<List<int>> GetUserIdsWithSuccessfulPaymentsAsync() => await _paymentDAO.GetUserIdsWithSuccessfulPaymentsAsync();
    }
}
