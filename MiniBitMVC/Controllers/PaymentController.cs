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
    public async Task<IActionResult> payOSTransferHandler(WebhookType body)
    {
        try
        {
            WebhookData data = _payOS.verifyPaymentWebhookData(body);

            _logger.LogInformation("Webhook Data: {WebhookData}", data);

            if (data.description == "Ma giao dich thu nghiem" || data.description == "VQRIO123")
            {
                return Ok(new Response(0, "Ok", null));
            }
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                _logger.LogWarning("Không tìm thấy UserId trong session.");
                return Unauthorized("Không tìm thấy UserId trong session.");
            }
            var payment = new Payment
            {
                UserId = userId.Value,
                Amount = data.amount,
                Currency = data.currency,
                PaymentDate = DateTime.UtcNow,
                Method = "payOS",
            };
            _logger.LogInformation("Thanh toán được tạo: {PaymentAmount} {PaymentCurrency}", payment.Amount, payment.Currency);
            await _paymentService.SavePaymentAsync(payment);
            _logger.LogInformation("Thanh toán đã được lưu thành công.");
            return Ok(new Response(0, "Ok", null));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Ok(new Response(-1, "fail", null));
        }

    }
}