using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Business.Interfaces;

namespace MyShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.DisplayAllUsers();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            await _userService.CustomerToAdmin(userId);
            TempData["Update"] = "User promoted to Admin.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DemoteToCustomer(string userId)
        {
            await _userService.AdminToCustomer(userId);
            TempData["Update"] = "User demoted to Customer.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockAccount(string userId)
        {
            await _userService.LockUserAccount(userId);
            TempData["Update"] = "User account locked.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLockAccount(string userId)
        {
            await _userService.UnLockUserAccount(userId);
            TempData["Update"] = "User account unlocked.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userService.DeleteAccount(userId);
            TempData["Delete"] = "User account deleted.";
            return RedirectToAction(nameof(Index));
        }

    }
}
