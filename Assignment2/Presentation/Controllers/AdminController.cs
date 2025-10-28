using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModel;

namespace Presentation.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISystemAccountRepo _systemAccountRepo;
        private readonly IArticleRepo _articleRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly FunewsManagementContext _db;

        public AdminController(ISystemAccountRepo systemAccountRepo, IArticleRepo articleRepo,
            FunewsManagementContext db, ICategoryRepo categoryRepo)
        {
            _systemAccountRepo = systemAccountRepo;
            _articleRepo = articleRepo;
            _db = db;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> ManagerAccount()
        {
            var accounts = await _systemAccountRepo.AllAsync();

            var vm = new AccountPageVM
            {
                SystemAccounts = accounts
            };

            return View(vm);
        }
    }
}
