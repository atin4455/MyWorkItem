using Microsoft.AspNetCore.Mvc;
using MyWorkItem.Services;

namespace MyWorkItem.Controllers
{
    public class AuthController(IAuthService authService) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "WorkItems");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string account, string password)
        {
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "帳號密碼不可空白。";
                return View();
            }

            var user = await authService.ValidateUserAsync(account, password);
            if (user == null)
            {
                ViewBag.Error = "帳號或密碼錯誤。";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Account", user.Account);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "WorkItems");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
