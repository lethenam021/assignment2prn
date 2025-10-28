using DataAccess.DTO;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ArticleServices : IArticleRepo
    {
        private readonly FunewsManagementContext _db;

        public ArticleServices(FunewsManagementContext db)
        {
            _db = db;
        }

        public async Task<List<NewsArticle>> AllAsync()
        {
            return await _db.NewsArticles.Where(x=>x.NewsStatus==true).ToListAsync();
        }
        public async Task<List<NewsArticle>> AllByUpdateAsync(short id)
        {
            return await _db.NewsArticles.Where(x =>x.UpdatedById==id).ToListAsync();
        }
        public async Task<NewsArticle?> GetAsync(string id)
        {
            return await _db.NewsArticles.FindAsync(id);
        }

        public async Task AddAsync(NewsArticle article)
        {
            await _db.NewsArticles.AddAsync(article);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(NewsArticle article)
        {
            _db.NewsArticles.Update(article);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var article = await _db.NewsArticles.FindAsync(id);
            if (article == null)
                throw new KeyNotFoundException("Không tìm thấy bài viết.");

            var hasTags = await _db.Entry(article)
                .Collection(a => a.Tags)
                .Query()
                .AnyAsync();

            if (hasTags)
                throw new InvalidOperationException("Bài viết có Tag liên kết, không thể xóa.");

            _db.NewsArticles.Remove(article);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<NewsArticle>> FilterAsync(NewsFilterDto filter)
        {
            var query = _db.NewsArticles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                query = query.Where(x => x.NewsTitle.Contains(filter.Keyword) ||
                                         x.NewsSource.Contains(filter.Keyword));

            if (!string.IsNullOrWhiteSpace(filter.Category))
                query = query.Where(x => x.NewsSource == filter.Category);

            if (filter.From.HasValue)
            {
                if (filter.DateField == "created")
                    query = query.Where(x => x.CreatedDate >= filter.From.Value);
                else
                    query = query.Where(x => x.ModifiedDate >= filter.From.Value);
            }

            if (filter.To.HasValue)
            {
                if (filter.DateField == "created")
                    query = query.Where(x => x.CreatedDate <= filter.To.Value);
                else
                    query = query.Where(x => x.ModifiedDate <= filter.To.Value);
            }

            // Sort
            query = filter.Sort switch
            {
                "created_asc" => query.OrderBy(x => x.CreatedDate),
                "created_desc" => query.OrderByDescending(x => x.CreatedDate),
                "title_asc" => query.OrderBy(x => x.NewsTitle),
                "title_desc" => query.OrderByDescending(x => x.NewsTitle),
                "verified_asc" => query.OrderBy(x => x.ModifiedDate),
                "verified_desc" => query.OrderByDescending(x => x.ModifiedDate),
                _ => query.OrderByDescending(x => x.CreatedDate)
            };

            return await query.ToListAsync();
        }
    }

    }

