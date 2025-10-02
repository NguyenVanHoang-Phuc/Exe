using BusinessObject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniBitMVC.ViewModels;
using Service;
using System.Security.Claims;

namespace MiniBitMVC.Controllers
{
    public class AccountController : Controller
    {

        private readonly IUserService _userService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IBudgetService _budgetService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger _logger;

        public AccountController(IUserService userService, IUserSubscriptionService userSubscriptionService, IBudgetService budgetService, ITransactionService transactionService)
        {
            _userService = userService;
            _userSubscriptionService = userSubscriptionService;
            _budgetService = budgetService;
            _transactionService = transactionService;
        }
        public IActionResult HomePage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActivatePremium(int userId, int planId, int months, decimal amount)
        {
            try
            {
                await _userSubscriptionService.UpsertActivateAsync(userId, planId, months, amount);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("HomePage");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginFormViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginFormViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var email = model.Email.Trim().ToLower();

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null || (user.PasswordHash ?? string.Empty) != (model.Password ?? string.Empty))
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng.";
                return View(model);
            }
            await SignInLocalAsync(user);

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Name);
            var isPremium = await _userSubscriptionService.HasActiveAsync(user.UserId);
            HttpContext.Session.SetInt32("IsPremium", isPremium ? 1 : 0);
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInLocalAsync(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Name, user.Name ?? string.Empty)
    };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = true, AllowRefresh = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var props = new AuthenticationProperties
            {
                // Sau khi middleware xử lý callback tại /signin-google,
                // nó sẽ redirect về đây:
                RedirectUri = Url.Action("GoogleCallback", "Account", new { returnUrl })
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (result?.Principal == null)
                return RedirectToAction(nameof(Login));
            var email = result.Principal.FindFirstValue(ClaimTypes.Email)
                   ?? result.Principal.FindFirstValue("email");
            var name = result.Principal.FindFirstValue(ClaimTypes.Name)
                        ?? result.Principal.FindFirstValue("name");

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Google không trả về email.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = await _userService.CreateUserFromGoogleAsync(email, name);
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp(string? returnUrl = null)
        {
            return View(new SignUpViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim();
            if (await _userService.EmailExistsAsync(email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã tồn tại.");
                return View(model);
            }

            var user = new User
            {
                RoleId = 1,
                Email = email,
                PasswordHash = model.Password,
                Name = model.Name,
                CreatedAt = DateTime.UtcNow
            };

            user = await _userService.CreateAsync(user);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            // Kiểm tra xem người dùng có trạng thái "active" hay không
            var isPremium = await _userSubscriptionService.HasActiveAsync(userId.Value);

            // Truyền thông tin vào ViewBag hoặc session để hiển thị trên header
            ViewBag.IsPremium = isPremium;

            return View();
        }

    }
}
