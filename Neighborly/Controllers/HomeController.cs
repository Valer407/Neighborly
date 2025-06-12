using Microsoft.AspNetCore.Mvc;
using Neighborly.Models;
using Neighborly.Models.DBModels;
using Neighborly.Data;
using System.Diagnostics;

namespace Neighborly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NeighborlyContext _context;

        public HomeController(ILogger<HomeController> logger, NeighborlyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.IconSvg))
                {
                    category.IconSvg = Icons.GetIcon(category.Icon);
                }
            }

            return View(categories);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new Neighborly.Models.ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}