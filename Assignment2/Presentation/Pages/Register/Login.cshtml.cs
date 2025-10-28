using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Presentation.Pages.Register
{
    public class LoginModel : PageModel
    {
        private readonly ISystemAccountRepo _accounts;

        public LoginModel(ISystemAccountRepo accounts)
        {
            _accounts = accounts;
        }

        public void OnGet() { }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            var user = await _accounts.CheckLoginAsync(email, password);
            if (user == null)
            {
                TempData["AlertMessage"] = "Sai email hoặc mật khẩu. Vui lòng đăng ký tài khoản mới!";
                return Page();
            }

            // Lưu Session (nếu muốn dùng thêm)
            HttpContext.Session.SetInt32("UserId", user.AccountId);
            HttpContext.Session.SetInt32("UserRole", user.AccountRole);

            // ✅ Đăng nhập cookie có claim số role
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
        new Claim(ClaimTypes.Email, user.AccountEmail),
        new Claim(ClaimTypes.Role, user.AccountRole.ToString()) // lưu số
    };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            if (user.AccountRole == 3)
            {
                return LocalRedirect("/account");
            }
            else
            {
                return LocalRedirect("/category");
            }
        }
    }
}
