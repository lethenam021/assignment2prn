using DataAccess.Models;

namespace Presentation.ViewModel
{
    public class ReportPageVM
    {
        public List<DataAccess.Models.NewsArticle> NewsArticles { get; set; } = new();
        public List<DataAccess.Models.Category> Categories { get; set; } = new();
        public List<SystemAccount?> Authors { get; set; } = new();
        public List<bool?>? StatusList { get; set; }
        public List<NewArticleVM> Articles { get; set; } = new();

    }
}
