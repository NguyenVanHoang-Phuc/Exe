using BusinessObject.Models;
using DataObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataObject.PayOSDAO;
namespace Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOSDAO _payOSDAO;
        private readonly IPaymentRepository _paymentRepository;
        public PaymentService(PayOSDAO payOSDAO, IPaymentRepository paymentRepository)
        {
            _payOSDAO = payOSDAO;
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResult> CreatePaymentAsync(Payment payment)
        {
            var result = await _payOSDAO.CreatePaymentAsync(payment);

            if (result.IsSuccess)
            {
                // Nếu thanh toán thành công, lưu thông tin thanh toán vào cơ sở dữ liệu
                await _paymentRepository.SavePaymentAsync(payment);
            }

            return result;
        }

        public async Task SavePaymentAsync(Payment payment) => await _paymentRepository.SavePaymentAsync(payment);

        public async Task<int> AddPaymentAsync(int userId, decimal amount, string status, string method) => await _paymentRepository.AddPaymentAsync(userId, amount, status, method);
        public async Task<PaymentResult> GetPaymentStatusAsync(string transactionId) => await _paymentRepository.GetPaymentStatusAsync(transactionId);
        public async Task<List<int>> GetUserIdsWithSuccessfulPaymentsAsync() => await _paymentRepository.GetUserIdsWithSuccessfulPaymentsAsync();
    }

}
