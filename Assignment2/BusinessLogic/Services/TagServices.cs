using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class TagServices: ITagRepo
    {
        private readonly FunewsManagementContext _db;

        public TagServices(FunewsManagementContext db)
        {
            _db = db;
        }

        public async Task<List<Tag>> AllAsync()
        {
            return await _db.Tags.ToListAsync();
        }

        public async Task<Tag?> GetAsync(int id)
        {
            return await _db.Tags.FindAsync(id);
        }

        public async Task AddAsync(Tag tag)
        {
            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tag tag)
        {
            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tag = await _db.Tags.FindAsync(id);
            if (tag != null)
            {
                _db.Tags.Remove(tag);
                await _db.SaveChangesAsync();
            }
        }


    }
}
