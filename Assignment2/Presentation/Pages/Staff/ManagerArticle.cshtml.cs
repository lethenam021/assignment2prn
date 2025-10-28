using System.Linq; // ensure this is present
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages.Staff
{
    public class ManagerArticleModel : PageModel
    {
        private readonly IArticleRepo _articleRepo;
        public ManagerArticleModel(IArticleRepo articleRepo) => _articleRepo = articleRepo;

        // If you want JSON via /article?handler=List
        public async Task<IActionResult> OnGetList()
        {
            var items = await _articleRepo.AllAsync() ?? Enumerable.Empty<NewsArticle>();

            return new JsonResult(items.Select(a => new
            {
                id = a.NewsArticleId,
                title = a.NewsTitle,
                content = a.NewsContent,
                newsource = a.NewsSource,
                headline = a.Headline,
                created = a.CreatedDate,
            }));
        }


        // If the page also renders a list
        public List<NewsArticle> Articles { get; private set; } = new();

        public async Task OnGet()
        {
            Articles = (await _articleRepo.AllAsync())?.ToList() ?? new();
        }
    }
}
