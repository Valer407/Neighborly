using Microsoft.AspNetCore.Mvc;
using Neighborly.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;

namespace Neighborly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "categories.json");
            var jsonData = System.IO.File.ReadAllText(jsonFilePath);
            var categories = JsonSerializer.Deserialize<List<Category>>(jsonData);

            foreach (var category in categories)
            {
                category.IconSvg = Icons.GetIcon(category.Icon);
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
