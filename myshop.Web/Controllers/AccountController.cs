using Microsoft.AspNetCore.Mvc;
using MyShop.Business.DTOs;
using MyShop.Business.Interfaces;

namespace MyShop.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _accountService.LoginAsync(dto);

            if (result.Succeeded)
                return RedirectToAction("Index", "Product");

            if (result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "Account locked out.");
            else
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _accountService.RegisterAsync(dto);

            if (result.Succeeded)
            {
                TempData["Success"] = "Account created successfully.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(dto);
        }

        [HttpGet]
        public IActionResult AccessDenied(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                var segments = returnUrl.Trim('/').Split('/');
                var controller = segments.Length > 0 ? segments[0] : "Home";

                TempData["Error"] = "You don't have permission to do that.";
                return RedirectToAction("Index", controller);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
