namespace Presentation.ViewModel
{
    public class NewArticlePageVM
    {
        public List<DataAccess.Models.NewsArticle> NewsArticles { get; set; } = new();
        public List<NewArticleVM> Articles { get; set; } = new();

        public NewArticleVM ArticleVM { get; set; } = new();
    }
}
