using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class CategoryServices : ICategoryRepo
    {
        private readonly FunewsManagementContext _db;

        public CategoryServices(FunewsManagementContext db)
        {
            _db = db;
        }

        public async Task<List<Category>> AllAsync()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<Category?> GetAsync(short id)
        {
            return await _db.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
            }
        }
    }
}
