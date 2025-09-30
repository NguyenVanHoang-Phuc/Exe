using BusinessObject.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        public async Task SavePaymentAsync(Payment payment) => await _paymentRepository.SavePaymentAsync(payment);
        public async Task<int> AddPaymentAsync(int userId, int planId, decimal amount, string status, string method)
            => await _paymentRepository.AddPaymentAsync(userId, planId, amount, status, method);
    }

}
