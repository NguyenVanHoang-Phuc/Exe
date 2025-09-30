namespace MiniBitMVC.Controllers;

using BusinessObject.Models;
using DataObject;
using Microsoft.AspNetCore.Mvc;
using MiniBitMVC.Types;
using Net.payOS;
using Net.payOS.Types;
using Service;

[Route("[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly PayOS _payOS;
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;
    private readonly ILogger<PaymentController> _logger;
    public PaymentController(PayOS payOS, IPaymentService paymentService,IUserService userService, ILogger<PaymentController> logger)
    {
        _payOS = payOS;
        _paymentService = paymentService;
        _userService = userService;
        _logger = logger;
    }
    [HttpPost("payos_transfer_handler")]
    public async Task<IActionResult> payOSTransferHandler([FromBody] WebhookData data)
    {
        try
        {
            // Log dữ liệu nhận được từ webhook để kiểm tra
            _logger.LogInformation("Webhook Data received: {@WebhookData}", data);

            // Lấy UserId từ session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                _logger.LogWarning("Không tìm thấy UserId trong session.");
                return Unauthorized("Không tìm thấy UserId trong session.");
            }

            // Tạo đối tượng Payment từ dữ liệu webhook
            var payment = new Payment
            {
                UserId = userId.Value,           // Lấy UserId từ session
                Amount = data.amount,            // Số tiền thanh toán
                Currency = data.currency,        // Loại tiền tệ
                PaymentDate = DateTime.UtcNow,   // Ngày thanh toán
                Method = "payOS",                // Phương thức thanh toán
            };

            // Log thanh toán được tạo
            _logger.LogInformation("Thanh toán được tạo: {PaymentAmount} {PaymentCurrency} cho UserId: {UserId}",
                payment.Amount, payment.Currency, userId);

            // Lưu thanh toán vào database
            await _paymentService.SavePaymentAsync(payment);

            // Log thông báo thanh toán đã được lưu
            _logger.LogInformation("Thanh toán đã được lưu thành công.");

            return Ok(new Response(0, "Payment processed successfully.", null));
        }
        catch (Exception e)
        {
            // Log lỗi nếu có
            _logger.LogError(e, "Error occurred while processing PayOS webhook.");
            return StatusCode(500, "Internal server error");
        }
    }

}
