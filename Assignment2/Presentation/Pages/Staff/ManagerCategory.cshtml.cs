using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static Presentation.Pages.Admin.ManagerAccountModel;

namespace Presentation.Pages.Staff
{
    public class ManagerCategoryModel : PageModel
    {
        private readonly FunewsManagementContext _db;
        private readonly IHubContext<AppHub> _hub;
        private readonly ICategoryRepo _repo;

        public ManagerCategoryModel(FunewsManagementContext db, IHubContext<AppHub> hub
            , ICategoryRepo repo)
        {
            _db = db; _hub = hub; _repo = repo;
        }
        // phát token để JS gửi header RequestVerificationToken
        public string AntiForgeryToken => HttpContext.RequestServices
            .GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>()
            .GetAndStoreTokens(HttpContext).RequestToken!;

        public List<Category> Categories { get; private set; } = new(); 

        public async Task<IActionResult> OnGetList()
        {
            var items = await _repo.AllAsync();
            return new JsonResult(items.Select(a =>
            new {
                id = a.CategoryId,
                categoryname = a.CategoryName,
                categorydescription = a.CategoryDesciption,
                categoryparent = a.ParentCategoryId,
                isactive=a.IsActive
            }));
        }

        // ✅ record DTO để nhận dữ liệu từ JSON
        public record CreateDto(string CategoryName, string? CategoryDescription, bool IsActive);

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostCreateCategory([FromBody] CreateDto dto)
        {
            if (dto == null)
            {
                Console.WriteLine("⚠️ DEBUG: dto is null");
                return new JsonResult(new { ok = false, error = "Category data is null" });
            }

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                Console.WriteLine("⚠️ DEBUG: CategoryName is empty");
                return new JsonResult(new { ok = false, error = "Invalid category data" });
            }

            try
            {
                // Nếu CategoryId trong DB là identity thì bỏ phần này
               

                var category = new Category
                {
                    CategoryName = dto.CategoryName.Trim(),
                    CategoryDesciption = dto.CategoryDescription?.Trim(),
                    IsActive = dto.IsActive
                };

                _db.Categories.Add(category);
                await _db.SaveChangesAsync();

                await _hub.Clients.All.SendAsync("notice", $"🆕 New category '{category.CategoryName}' added!");

                Console.WriteLine($"✅ DEBUG: Category '{category.CategoryName}' created successfully (ID={category.CategoryId})");

                return new JsonResult(new { ok = true, id = category.CategoryId });
            }
          
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR saving category: " + ex);
                if (ex.InnerException != null)
                    Console.WriteLine("🔍 InnerException: " + ex.InnerException.Message);

                return new JsonResult(new { ok = false, error = ex.InnerException?.Message ?? ex.Message });
            }

        }
 
        public record IdDto(short Id);

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDelete([FromBody] IdDto dto)
        {
            var category = await _db.Categories.FindAsync(dto.Id);
            if (category == null)
                return new JsonResult(new { ok = false, error = "Not found" });

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            // ✅ Phát sự kiện real-time tới tất cả client
            await _hub.Clients.All.SendAsync(
                "category:updated",                     // 🔹 phải trùng với client-side
                $"❌ Category '{category.CategoryName}' deleted."
            );

            return new JsonResult(new { ok = true });
        }

    }
}
