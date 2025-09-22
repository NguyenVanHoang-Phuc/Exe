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

        public AccountController(IUserService userService, IUserSubscriptionService userSubscriptionService)
        {
            _userService = userService;
            _userSubscriptionService = userSubscriptionService;
        }
        public IActionResult HomePage()
        {
            return View();
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

            var isPremium = await _userSubscriptionService.HasActiveAsync(user.UserId);

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
            await HttpContext.SignOutAsync();
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

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> Profile()
        //{
        //    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //    var user = await _userService.GetUserByIdAsync(userId);

        //    if (user == null) return NotFound();

        //    var userSubscription = await _userSubscriptionService.HasActiveAsync(userId);
        //    bool isPremium = userSubscription != null && userSubscription.Ex
        //    return View();
        //}
    }
}
