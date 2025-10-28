using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ISystemAccountRepo _accounts;
        public RegisterController(ISystemAccountRepo accounts)
        {
            _accounts = accounts;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _accounts.CheckLoginAsync(email, password);
            if (user == null)
            {
                TempData["AlertMessage"] = "Sai email hoặc mật khẩu. Vui lòng đăng ký tài khoản mới!";
                return RedirectToAction("Login", "Register");
            }

            HttpContext.Session.SetInt32("UserId", user.AccountId);
            HttpContext.Session.SetInt32("UserRole", user.AccountRole);

            return LocalRedirect("/account");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}
