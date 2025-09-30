using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IPaymentRepository
    {
        Task SavePaymentAsync(Payment payment);
        Task<int> AddPaymentAsync(int userId, int planId, decimal amount, string status, string method);
    }
}
