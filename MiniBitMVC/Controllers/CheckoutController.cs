using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using Service;
using System.Threading.Tasks;

namespace MiniBitMVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PayOS _payOS;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserSubscriptionService _userSubscriptionService;

        public CheckoutController(PayOS payOS, IHttpContextAccessor httpContextAccessor, IUserSubscriptionService userSubscriptionService)
        {
            _payOS = payOS;
            _httpContextAccessor = httpContextAccessor;
            _userSubscriptionService = userSubscriptionService;
        }

        [HttpGet("/cancel")]
        public IActionResult Cancel()
        {
            // Trả về trang HTML có tên "MyView.cshtml"
            return View("cancel");
        }
        [HttpGet("/success")]
        public async Task<IActionResult> Success(int userId, int planId, int months, decimal amount)
        {
            try
            {
                await _userSubscriptionService.UpsertActivateAsync(userId, planId, months, amount);

                HttpContext.Session.SetString("IsPremium", "true");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost("/create-payment-link")]
        public async Task<IActionResult> Checkout(int userId, int planId, int months)
        {
            try
            {
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                ItemData item = new ItemData("Thanh toán premium", 1, 2000);
                List<ItemData> items = new List<ItemData> { item };

                // Get the current request's base URL
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                PaymentData paymentData = new PaymentData(
                    orderCode,
                    2000,
                    "Thanh toan don hang",
                    items,
                    $"{baseUrl}/cancel",
                    $"{baseUrl}/success"
                );

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                return Redirect(createPayment.checkoutUrl);
            }
            catch (System.Exception exception)
            {
                Console.WriteLine(exception);
                return Redirect("/");
            }
        }
    }
}