using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IPaymentService
    {
        Task SavePaymentAsync(BusinessObject.Models.Payment payment);
        Task<int> AddPaymentAsync(int userId, int planId, decimal amount, string status, string method);
    }
}
