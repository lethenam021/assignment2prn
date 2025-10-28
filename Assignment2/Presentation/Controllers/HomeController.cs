using DataAccess.DTO;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleRepo _articleRepo;

        public HomeController(IArticleRepo articleRepo, ILogger<HomeController> logger)
        {
            _articleRepo = articleRepo;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            var allArticles = await _articleRepo.AllAsync();
            return View(allArticles);
        }
        public async Task<IActionResult> Filter([FromQuery] NewsFilterDto filter)
        {
            var result = await _articleRepo.FilterAsync(filter);
            return View("Index", result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
