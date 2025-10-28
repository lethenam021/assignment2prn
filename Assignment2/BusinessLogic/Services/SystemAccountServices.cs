using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class SystemAccountServices : ISystemAccountRepo
    {
        private readonly FunewsManagementContext _db;

        public SystemAccountServices(FunewsManagementContext db)
        {
            _db = db;
        }
        public async Task<SystemAccount?> CheckLoginAsync(string email, string password)
        {
            return await _db.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
        }

        public async Task<List<SystemAccount>> AllAsync()
        {
            return await _db.SystemAccounts.ToListAsync();
        }

        public async Task<SystemAccount?> GetAsync(short id)
        {
            return await _db.SystemAccounts.FindAsync(id);
        }

        public async Task AddAsync(SystemAccount account)
        {
            _db.SystemAccounts.Add(account);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(SystemAccount account)
        {
            _db.SystemAccounts.Update(account);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            var account = await _db.SystemAccounts.FindAsync(id);
            if (account != null)
            {
                _db.SystemAccounts.Remove(account);
                await _db.SaveChangesAsync();
            }
        }

      
    }
}
