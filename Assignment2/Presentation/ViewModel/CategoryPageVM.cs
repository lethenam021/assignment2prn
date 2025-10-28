using DataAccess.Models;
using Microsoft.Extensions.Hosting;

namespace Presentation.ViewModel
{
    public class CategoryPageVM
    {
        public List<DataAccess.Models.Category> Categories { get; set; } = new();
        public CategoryVM CategoryVM { get; set; } = new();
        public List<NewsArticle>? NewArticle { get; set; }

    }
}
