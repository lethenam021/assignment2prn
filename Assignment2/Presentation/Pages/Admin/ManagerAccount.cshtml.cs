using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Pages.Admin
{
    public class ManagerAccountModel : PageModel
    {
        private readonly FunewsManagementContext _db;
        private readonly IHubContext<AppHub> _hub;
        private readonly ISystemAccountRepo _repo;

        public ManagerAccountModel(FunewsManagementContext db, IHubContext<AppHub> hub
            , ISystemAccountRepo repo)
        {
            _db = db; _hub = hub; _repo = repo;
        }

        // phát token để JS gửi header RequestVerificationToken
        public string AntiForgeryToken => HttpContext.RequestServices
            .GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>()
            .GetAndStoreTokens(HttpContext).RequestToken!;

        public List<SystemAccount> SystemAccounts { get; private set; } = new(); // ✅ KHỞI TẠO

        public async Task<IActionResult> OnGetList()
        {
            var items = await _repo.AllAsync();
            return new JsonResult(items.Select(a => 
            new {
                id = a.AccountId,
                userName = a.AccountName,
                email = a.AccountEmail,
                role = a.AccountRole
            }));
        }


        // POST /Accounts/Index?handler=Create  (JSON body)
        public async Task<IActionResult> OnPostCreate([FromBody] CreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Email))
                return new JsonResult(new { ok = false, error = "Invalid data" });

            int nextId = _db.SystemAccounts.Any()
                ? _db.SystemAccounts.Max(a => a.AccountId) + 1
                : 1;

            var acc = new SystemAccount
            {
                AccountId = (short)nextId,
                AccountName = dto.UserName.Trim(),
                AccountEmail = dto.Email.Trim(),
                AccountRole = dto.Role
            };

            _db.SystemAccounts.Add(acc);
            await _db.SaveChangesAsync();

            await _hub.Clients.Group("staff")
                .SendAsync("notice", $"Admin has added a new account: {acc.AccountName}");

            await _hub.Clients.Group("admin")
                .SendAsync("notice", $"Admin has added a new account: {acc.AccountName}");

            return new JsonResult(new { ok = true, id = acc.AccountId });
        }


        // POST /Accounts/Index?handler=Edit
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostEdit([FromBody] EditDto dto)
        {
            var acc = await _db.SystemAccounts.FindAsync(dto.Id);
            if (acc == null) return new JsonResult(new { ok = false, error = "Not found" });

            acc.AccountName = dto.UserName?.Trim() ?? acc.AccountName;
            acc.AccountEmail = dto.Email?.Trim() ?? acc.AccountEmail;
            acc.AccountRole = dto.Role;
            await _db.SaveChangesAsync();
            return new JsonResult(new { ok = true });
        }

        //// POST /Accounts/Index?handler=ToggleActive
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> OnPostToggleActive([FromBody] IdDto dto)
        //{
        //    var acc = await _db.SystemAccounts.FindAsync(dto.Id);
        //    if (acc == null) return new JsonResult(new { ok = false, error = "Not found" });

        //    acc.I = !acc.IsActive;
        //    await _db.SaveChangesAsync();

        //    if (!acc.IsActive)
        //    {
        //        // Kick tất cả session đang online của user này
        //        await _hub.Clients.Group($"user:{acc.AccountId}")
        //            .SendAsync("forceLogout", "Your account has been deactivated.");
        //        // Nếu dùng Identity: await _userManager.UpdateSecurityStampAsync(user);
        //    }
        //    return new JsonResult(new { ok = true });
        //}

        // POST /Accounts/Index?handler=Delete
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDelete([FromBody] IdDto dto)
        {
            var acc = await _db.SystemAccounts.FindAsync(dto.Id);
            if (acc == null) 
                return new JsonResult(new { ok = false, error = "Not found" });

            _db.SystemAccounts.Remove(acc);
            await _db.SaveChangesAsync();

            await _hub.Clients.Group($"user:{acc.AccountId}")
                .SendAsync("forceLogout", "Your account has been deleted.");
            return new JsonResult(new { ok = true });
        }

        public record CreateDto(string UserName, string Email, int Role);
        public record EditDto(string Id, string UserName, string Email, int Role);
        public record IdDto(short Id);
    }
}

