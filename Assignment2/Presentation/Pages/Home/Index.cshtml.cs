using BusinessLogic.Services;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

// ✅ ADD: namespace chứa DTO và Store tạm
using Presentation.Models;      // CommentDto (nếu bạn để ở đây)

namespace Presentation.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly IArticleRepo _articleServices;
        private readonly ICategoryRepo _repo;
        private readonly IHubContext<AppHub> _hub;

        public List<NewsArticle> Articles { get; private set; } = null!;

        public IndexModel(IArticleRepo articleServices, ICategoryRepo repo, IHubContext<AppHub> hub)
        {
            _articleServices = articleServices;
            _repo = repo;
            _hub = hub;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Articles = await _articleServices.AllAsync();

            // ✅ ADD: gắn bình luận từ store tạm vào từng bài (nếu NewsArticle có property Comments)
            foreach (var a in Articles)
            {
                // nếu NewsArticle có List<CommentDto> Comments:
                a.Comments = CommentMemoryStore.GetByArticle((a.NewsArticleId));
            }

            return Page();
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> OnGetCList()
        {
            var items = await _repo.AllAsync();
            return new JsonResult(items.Select(a => new
            {
                id = a.CategoryId,
                categoryname = a.CategoryName,
            }));
        }

        // =========================
        // ✅ Bình luận: ADD/UPDATE/DELETE
        // =========================
         [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostAddComment([FromBody] AddCommentRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.content))
                return new JsonResult(new { success = false, message = "Nội dung trống." });

            var cmt = CommentMemoryStore.AddComment(req.articleId, req.content.Trim(),
                string.IsNullOrWhiteSpace(req.author) ? "Ẩn danh" : req.author!);

            await _hub.Clients.All.SendAsync("ReceiveCommentAdded", req.articleId, cmt);
            return new JsonResult(new { success = cmt });
        }

        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostUpdateComment([FromBody] UpdateCommentRequest req)
        {
            if (req==null||string.IsNullOrWhiteSpace(req.content))
                return new JsonResult(new { success = false, message = "Nội dung trống." });

            var ok = CommentMemoryStore.UpdateComment(req.articleId, req.commentId, req.content.Trim());

            if (ok)
            {
                var updated = CommentMemoryStore.GetByArticle(req.articleId)
                                                .FirstOrDefault(c => c.Id == req.commentId);
                // ✅ Gửi thông báo cập nhật
                await _hub.Clients.All.SendAsync("ReceiveCommentUpdated", req.articleId, updated);
            }

            return new JsonResult(new { success = ok });
        }

        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostDeleteComment([FromBody] DeleteCommentRequest req)
        {
            var ok = CommentMemoryStore.DeleteComment(req.articleId, req.commentId);

            if (ok)
            {
                // ✅ Gửi thông báo xóa
                await _hub.Clients.All.SendAsync("ReceiveCommentDeleted", req.articleId, req.commentId);
            }

            return new JsonResult(new { success = ok });
        }


        // (Optional) Lấy danh sách comment riêng qua AJAX
        public JsonResult OnGetComments(string articleId)
        {
            var list = CommentMemoryStore.GetByArticle(articleId);
            return new JsonResult(new { success = true, data = list });
        }

        public class AddCommentRequest
        {
            public string articleId { get; set; } = "";
            public string content { get; set; } = "";
            public string? author { get; set; } = "";
        }

        public class UpdateCommentRequest
        {
            public string articleId { get; set; } = "";
            public int commentId { get; set; }
            public string content { get; set; } = "";
        }

        public class DeleteCommentRequest
        {
            public string articleId { get; set; } = "";
            public int commentId { get; set; }
        }



    }
}
