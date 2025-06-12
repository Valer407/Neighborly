using Microsoft.AspNetCore.Mvc;
using Neighborly.Models;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Neighborly.Controllers
{
    public class ListingsController : Controller
    {
        private readonly NeighborlyContext _context;

        public ListingsController(NeighborlyContext context)
        {
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

        // GET: /Listings/NewListing
        [HttpGet]
        public IActionResult NewListing()
        {
            ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.ListingTypes = _context.Listing_Types.OrderBy(t => t.Name).ToList();
            return View();
        }

        // POST: /Listings/NewListing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewListing(Listings listing, string City, string District)
        {
            ModelState.Remove("CityId");
            ModelState.Remove("DistrictId");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
                ViewBag.ListingTypes = _context.Listing_Types.OrderBy(t => t.Name).ToList();
                return View(listing);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            bool categoryOk = _context.Categories.Any(c => c.CategoryId == listing.CategoryId);
            bool typeOk = _context.Listing_Types.Any(t => t.ListingTypeId == listing.ListingTypeId);

            City = City?.Trim();
            District = District?.Trim();

            if (!categoryOk || !typeOk || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(District))
            {
                ModelState.AddModelError(string.Empty, "Niepoprawne dane lokalizacji lub kategorii.");
                ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
                ViewBag.ListingTypes = _context.Listing_Types.OrderBy(t => t.Name).ToList();
                return View(listing);
            }

            var cityEntity = _context.Cities.FirstOrDefault(c => c.Name == City);
            if (cityEntity == null)
            {
                cityEntity = new Cities { Name = City };
                _context.Cities.Add(cityEntity);
                _context.SaveChanges();
            }

            var districtEntity = _context.Districts.FirstOrDefault(d => d.Name == District && d.CityId == cityEntity.CityId);
            if (districtEntity == null)
            {
                districtEntity = new Distircts { Name = District, CityId = cityEntity.CityId };
                _context.Districts.Add(districtEntity);
                _context.SaveChanges();
            }

            listing.CityId = cityEntity.CityId;
            listing.DistrictId = districtEntity.DistrictId;

            if (!TryValidateModel(listing))
            {
                ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
                ViewBag.ListingTypes = _context.Listing_Types.OrderBy(t => t.Name).ToList();
                return View(listing);
            }

            listing.UserId = user.UserId;
            listing.CreatedAt = DateTime.UtcNow;
            listing.UpdatedAt = DateTime.UtcNow;
            listing.Latitude = 0;
            listing.Longitude = 0;

            _context.Listings.Add(listing);
            _context.SaveChanges();

            return RedirectToAction("Index", "Listings");
        }
    }
}