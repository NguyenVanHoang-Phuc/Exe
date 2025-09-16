using Microsoft.AspNetCore.Mvc;

namespace MiniBitMVC.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult HomePage()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
