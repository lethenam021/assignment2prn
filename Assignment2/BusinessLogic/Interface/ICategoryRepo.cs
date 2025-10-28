using DataAccess.Models;
namespace DataAccess.Repositories
{
    public interface ICategoryRepo
    {
        Task<List<Category>> AllAsync();
        Task<Category?> GetAsync(short id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(short id);
    }
}
