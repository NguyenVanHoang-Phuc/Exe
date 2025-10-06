using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataObject.PayOSDAO;

namespace Repositories
{
    public interface IPaymentRepository
    {
        Task SavePaymentAsync(Payment payment);
        Task<int> AddPaymentAsync(int userId, decimal amount, string status, string method);
        Task<PaymentResult> GetPaymentStatusAsync(string transactionId);
        Task<List<int>> GetUserIdsWithSuccessfulPaymentsAsync();
    }
}
