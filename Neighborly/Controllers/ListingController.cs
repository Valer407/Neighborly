using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Neighborly.Models;

namespace Neighborly.Controllers
{
    public class ListingsController : Controller
    {
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

        // GET: /Listings/NewListing
        [HttpGet]
        public IActionResult NewListing()
        {
            return View();
        }

        // POST: /Listings/NewListing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewListing(NewListingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

 // TODO: Sprawdzenie czy użytkownik istnieje + zapisz ogłoszenie do bazy
            /*
            var listing = new Listing
            {
                Title = model.Title,
                Description = model.Description,
                Category = model.Category,
                Type = model.Type,
                City = model.City,
                District = model.District,
                CreatedAt = DateTime.Now,
                AuthorFirstName = "Test",
                AuthorLastName = "User",
                AuthorAvatarUrl = "/images/avatar.jpg",
                Rating = 4.9
            };

            _context.Listings.Add(listing);
            _context.SaveChanges();
            */

            return RedirectToAction("Index", "Listings");
        }
    }
}
