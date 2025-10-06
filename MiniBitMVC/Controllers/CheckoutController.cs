using BusinessObject.Models;
using DataObject;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Net.payOS;
using Net.payOS.Types;
using Service;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniBitMVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PayOS _payOS;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IPaymentService _paymentService;

        public CheckoutController(PayOS payOS, IHttpContextAccessor httpContextAccessor, IUserSubscriptionService userSubscriptionService, IPaymentService paymentService)
        {
            _payOS = payOS;
            _httpContextAccessor = httpContextAccessor;
            _userSubscriptionService = userSubscriptionService;
            _paymentService = paymentService;
        }

        [HttpGet("/cancel")]
        public IActionResult Cancel()
        {
            return View("cancel");
        }

        [HttpGet("/success")]
        public async Task<IActionResult> Success(
            [FromQuery] long orderCode,
            [FromQuery] string status,
            [FromQuery] bool cancel,
            [FromQuery] int userId,
            [FromQuery] int planId,
            [FromQuery] int months,
            [FromQuery] decimal amount)
        {
            try
            {
                var userIdFromCookie = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value; if (!int.TryParse(userIdFromCookie, out var uId))
                    return RedirectToAction("Login", "Account");
                bool isPaid = !cancel && string.Equals(status, "PAID", StringComparison.OrdinalIgnoreCase);

                try
                {
                    var info = await _payOS.getPaymentLinkInformation(orderCode);
                    if (!string.IsNullOrEmpty(info.status))
                        isPaid = string.Equals(info.status, "PAID", StringComparison.OrdinalIgnoreCase);
                    if (info.amount > 0 && amount <= 0) amount = info.amount;
                }
                catch {  }

                if (!isPaid) return View("cancel");

                await _paymentService.AddPaymentAsync(uId, amount, "Success", "PayOS");


                await _paymentService.SavePaymentAsync(new Payment
                {
                    UserId = uId,
                    Amount = amount,
                    Status = "Success",
                    Method = "PayOS",
                    Currency = "VND",
                    PaymentDate = DateTime.UtcNow
                });

                await _userSubscriptionService.UpsertActivateAsync(uId, months, amount);

                await RefreshPremiumClaimAsync(uId);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login", "Account");

                //ViewData["OrderCode"] = orderCode;
                //ViewData["Amount"] = amount;
                //return View("success"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        private async Task RefreshPremiumClaimAsync(int userId)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null) return;

            var existingClaim = identity.FindFirst("IsPremium");
            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);  // Xóa claim cũ nếu có
            }

            // Kiểm tra trạng thái premium của người dùng
            var userSubscription = await _userSubscriptionService.GetActiveSubscriptionAsync(userId);
            bool isPremium = userSubscription?.Status == "Active";  // Kiểm tra trạng thái "active"

            // Thêm claim "IsPremium" vào cookie
            identity.AddClaim(new Claim("IsPremium", isPremium ? "true" : "false"));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddDays(7) });
        }

        [HttpPost("/create-payment-link")]
        public async Task<IActionResult> Checkout(int userId, int planId, int months)
        {
            try
            {
                long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var items = new List<ItemData> { new ItemData("Thanh toán premium", 1, 2000) };

                var req = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{req.Scheme}://{req.Host}";

                var successUrl = QueryHelpers.AddQueryString($"{baseUrl}/success", new Dictionary<string, string?>
                {
                    ["planId"] = planId.ToString(),
                    ["months"] = months.ToString(),
                    ["amount"] = "2000"
                });

                var paymentData = new PaymentData(
                    orderCode,
                    2000,
                    "Thanh toan don hang",
                    items,
                    $"{baseUrl}/cancel",
                    successUrl
                );

                var createPayment = await _payOS.createPaymentLink(paymentData);
                return Redirect(createPayment.checkoutUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Redirect("/");
            }
        }
    }
}