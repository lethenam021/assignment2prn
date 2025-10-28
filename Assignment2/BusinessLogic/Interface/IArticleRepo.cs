using DataAccess.Models;
using DataAccess.DTO;
namespace DataAccess.Repositories
{
    public interface IArticleRepo
    {
        Task<List<NewsArticle>> AllAsync();
        Task<List<NewsArticle>> AllByUpdateAsync(short id);

        Task<NewsArticle?> GetAsync(string id);
        Task AddAsync(NewsArticle article);
        Task UpdateAsync(NewsArticle article);
        Task DeleteAsync(string id);
        Task<IEnumerable<NewsArticle>> FilterAsync(NewsFilterDto filter);

    }
}
