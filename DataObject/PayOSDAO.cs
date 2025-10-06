using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class PayOSDAO
    {
        private readonly HttpClient _httpClient;
        private readonly string _payOsApiUrl = "https://my.payos.vn/";  // Thay đổi URL nếu cần

        public PayOSDAO(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Gửi yêu cầu thanh toán đến PayOS
        public async Task<PaymentResult> CreatePaymentAsync(Payment payment)
        {
            var paymentData = new
            {
                user_id = payment.UserId,
                amount = payment.Amount,
                currency = payment.Currency,
                payment_method = payment.Method
            };

            try
            {
                // Gửi yêu cầu POST tới PayOS để xử lý thanh toán
                var response = await _httpClient.PostAsJsonAsync($"{_payOsApiUrl}payment", paymentData);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PayOSResponse>();
                return new PaymentResult
                {
                    IsSuccess = result.Status == "success",  // Kiểm tra trạng thái trả về từ PayOS
                    TransactionId = result.TransactionId    // Mã giao dịch từ PayOS
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing payment: {ex.Message}");
                return new PaymentResult { IsSuccess = false, ErrorMessage = ex.Message }; // Trả về lỗi
            }
        }

        // Kiểm tra trạng thái thanh toán từ PayOS
        public async Task<PaymentResult> GetPaymentStatusAsync(string transactionId)
        {
            try
            {
                // Gửi yêu cầu GET đến PayOS API để lấy trạng thái thanh toán
                var response = await _httpClient.GetAsync($"{_payOsApiUrl}payment/{transactionId}/status");

                // Kiểm tra nếu mã trạng thái HTTP không thành công, sẽ ném ngoại lệ
                response.EnsureSuccessStatusCode();

                // Đọc kết quả trả về từ PayOS
                var result = await response.Content.ReadFromJsonAsync<PayOSResponse>();

                // Kiểm tra dữ liệu trả về có hợp lệ không
                if (result == null)
                {
                    return new PaymentResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Không thể lấy thông tin thanh toán. Dữ liệu trả về từ PayOS không hợp lệ."
                    };
                }

                // Trả về kết quả thanh toán
                return new PaymentResult
                {
                    IsSuccess = result.Status == "success",  // Kiểm tra trạng thái thành công
                    TransactionId = result.TransactionId,    // Trả về mã giao dịch
                    ErrorMessage = result.Status == "success" ? null : "Thanh toán thất bại." // Thêm thông báo lỗi nếu cần
                };
            }
            catch (HttpRequestException httpEx)
            {
                // Xử lý lỗi HTTP nếu có
                Console.WriteLine($"HTTP error fetching payment status: {httpEx.Message}");
                return new PaymentResult { IsSuccess = false, ErrorMessage = "Lỗi kết nối tới PayOS." };
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                Console.WriteLine($"Error fetching payment status: {ex.Message}");
                return new PaymentResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }


        // Định nghĩa lớp PayOSResponse trong DAO
        public class PayOSResponse
        {
            public string Status { get; set; }  // Trạng thái thanh toán từ PayOS (success, failed)
            public string TransactionId { get; set; }  // Mã giao dịch từ PayOS
        }

        // Định nghĩa lớp PaymentResult trong DAO
        public class PaymentResult
        {
            public bool IsSuccess { get; set; }  // Thành công hay không
            public string TransactionId { get; set; }  // Mã giao dịch
            public string ErrorMessage { get; set; }  // Thông báo lỗi nếu có
            public string Status { get; set; }  // Thêm trạng thái thanh toán vào kết quả
        }
    }
}