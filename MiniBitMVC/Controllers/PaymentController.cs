namespace MiniBitMVC.Controllers;

using BusinessObject.Models;
using DataObject;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using MiniBitMVC.Types;
using Net.payOS;
using Net.payOS.Types;
using Service;
using System.Security.Claims;

[Route("[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly PayOS _payOS;
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;
    public PaymentController(PayOS payOS, IPaymentService paymentService, IUserService userService, ILogger<PaymentController> logger)
    {
        _payOS = payOS;
        _paymentService = paymentService;
        _userService = userService;
    }

    //[HttpPost("payos_transfer_handler")]
    //public async Task<IActionResult> payOSTransferHandler(WebhookType body)
    //{
    //    try
    //    {
    //        Console.WriteLine($"Webhook Data Received: {body}");

    //        WebhookData data = _payOS.verifyPaymentWebhookData(body);
    //        if (data == null)
    //        {
    //            Console.WriteLine("Dữ liệu webhook không hợp lệ.");
    //            return BadRequest("Invalid webhook data.");
    //        }
    //        Console.WriteLine($"Payment Data: {data}");

    //        var userIdFromCookie = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //        if (string.IsNullOrEmpty(userIdFromCookie))
    //        {
    //            Console.WriteLine("Không tìm thấy UserId trong cookie authentication.");
    //            return Unauthorized("Không tìm thấy UserId trong cookie authentication.");
    //        }
    //        var userExists = await _userService.GetUserByIdAsync(int.Parse(userIdFromCookie));
    //        if (userExists == null)
    //        {
    //            Console.WriteLine("Người dùng không tồn tại trong hệ thống.");
    //            return NotFound("Người dùng không tồn tại.");
    //        }
    //        var payment = new Payment
    //        {
    //            UserId = int.Parse(userIdFromCookie),
    //            Amount = data.amount,
    //            Currency = "VND", // Hoặc lấy từ thông tin người dùng
    //            PaymentDate = DateTime.UtcNow,
    //            Status = "paid", // "paid" hoặc "failed"
    //            Method = "payOS", // Ví dụ: "credit_card", "paypal", v.v.
    //        };
    //        await _paymentService.SavePaymentAsync(payment);

    //        Console.WriteLine($"Thanh toán được tạo với PaymentId: {payment.PaymentId}, {data.amount} {data.currency}");
    //        Console.WriteLine("Thanh toán đã được lưu thành công.");

    //        return Ok(new Response(0, "Payment processed successfully.", null));
    //    }
    //    catch (Exception e)
    //    {
    //        // Log lỗi nếu có
    //        Console.WriteLine($"Lỗi khi xử lý thanh toán: {e.Message}");
    //        return StatusCode(500, "Internal server error");
    //    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
    {
        if (payment == null)
        {
            return BadRequest("Invalid payment data.");
        }

        // Gọi service để tạo thanh toán qua PayOS
        var result = await _paymentService.CreatePaymentAsync(payment);

        if (result.IsSuccess)
        {
            // Lưu vào cơ sở dữ liệu nếu thành công
            await _paymentService.SavePaymentAsync(payment);

            return RedirectToAction("success", new { transactionId = result.TransactionId });
        }

        return BadRequest(new { success = false, message = result.ErrorMessage });
    }
    }
