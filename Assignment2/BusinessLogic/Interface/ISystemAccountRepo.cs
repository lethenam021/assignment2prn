using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface ISystemAccountRepo
    {
        Task<List<SystemAccount>> AllAsync();
        Task<SystemAccount?> GetAsync(short id);
        Task AddAsync(SystemAccount account);
        Task UpdateAsync(SystemAccount account);
        Task DeleteAsync(short id);
        Task<SystemAccount?> CheckLoginAsync(string email, string password);
    }
}
