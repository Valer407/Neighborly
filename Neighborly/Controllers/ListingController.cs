using Microsoft.AspNetCore.Mvc;
using Neighborly.Models;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Neighborly.Controllers
{
    public class ListingsController : Controller
    {
        private readonly NeighborlyContext _context;
        private readonly IWebHostEnvironment _env;

        public ListingsController(NeighborlyContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

            var listings = _context.Listings
                .Include(l => l.City)
                .Include(l => l.District)
                .Include(l => l.User)
                .Include(l => l.ListingType)
                .Select(l => new ListingCardViewModel
                {
                    Id = l.ListingId,
                    Title = l.Title,
                    Description = l.Description,
                    CreatedAt = l.CreatedAt,
                    Type = l.ListingType.Name == "Oferuję pomoc" ? "offer" : "request",
                    ImageUrl = _context.Listing_Images
                        .Where(i => i.ListingId == l.ListingId)
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.Url)
                        .FirstOrDefault(),
                    Location = new LocationViewModel
                    {
                        City = l.City.Name,
                        District = l.District.Name
                    },
                    User = new ListingCardUserViewModel
                    {
                        Id = l.User.UserId,
                        Name = l.User.FirstName + " " + l.User.LastName,
                        Avatar = l.User.AvatarUrl,
                        Rating = l.User.RatingAvg
                    }
                })
                .ToList();

            var viewModel = new ListingsIndexViewModel
            {
                Categories = categories,
                Listings = listings
            };

            return View(viewModel);
        }
        [HttpGet]
        [Route("ogloszenie/{id}")]
        public IActionResult Details(int id)
        {
            var listing = _context.Listings
                .Include(l => l.City)
                .Include(l => l.District)
                .Include(l => l.User)
                .Include(l => l.ListingType)
                .Include(l => l.Category)
                .FirstOrDefault(l => l.ListingId == id);

            if (listing == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            bool isFav = false;
            if (userId != null)
            {
                isFav = _context.Favourites.Any(f => f.UserId == userId.Value && f.ListingId == id);
            }
            var viewModel = new ListingDetailsViewModel
            {
                Listing = new ListingDetails
                {
                    Id = listing.ListingId,
                    Title = listing.Title,
                    Description = listing.Description,
                    CreatedAt = listing.CreatedAt,
                    Type = listing.ListingType.Name == "Oferuję pomoc" ? "offer" : "request",
                    Category = listing.Category.Name,
                    ImageUrl = _context.Listing_Images
                        .Where(i => i.ListingId == listing.ListingId)
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.Url)
                        .FirstOrDefault(),
                    Location = new LocationViewModel
                    {
                        City = listing.City.Name,
                        District = listing.District.Name
                    },
                    User = new ListingCardUserViewModel
                    {
                        Name = listing.User.FirstName + " " + listing.User.LastName,
                        Avatar = listing.User.AvatarUrl,
                        Rating = listing.User.RatingAvg
                    }
                },
                IsFavorite = isFav
            };

            return View(viewModel);
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
        public IActionResult NewListing(Listings listing, string City, string District, List<IFormFile> images)
        {
            ModelState.Remove("CityId");
            ModelState.Remove("DistrictId");
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("Category");
            ModelState.Remove("ListingType");
            ModelState.Remove("City");
            ModelState.Remove("District");

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
            listing.UserId = user.UserId;
            listing.User = user;

            if (!TryValidateModel(listing))
            {
                ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
                ViewBag.ListingTypes = _context.Listing_Types.OrderBy(t => t.Name).ToList();
                return View(listing);
            }

            listing.CreatedAt = DateTime.UtcNow;
            listing.UpdatedAt = DateTime.UtcNow;
            listing.Latitude = 0;
            listing.Longitude = 0;

            _context.Listings.Add(listing);
            _context.SaveChanges();

            if (images != null && images.Count > 0)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "listings");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                int order = 0;
                foreach (var file in images)
                {
                    if (file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png") continue;
                    if (file.Length > 5 * 1024 * 1024) continue; // 5MB limit

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var img = new Listing_images
                    {
                        ListingId = listing.ListingId,
                        Url = $"/uploads/listings/{fileName}",
                        SortOrder = order++
                    };
                    _context.Listing_Images.Add(img);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Listings");
        }
         [HttpPost]
        [Route("listings/favorite/{id}")]
        public IActionResult Favorite(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var fav = _context.Favourites.FirstOrDefault(f => f.UserId == userId.Value && f.ListingId == id);
            bool nowFav;
            if (fav == null)
            {
                fav = new Favourites
                {
                    UserId = userId.Value,
                    ListingId = id,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Favourites.Add(fav);
                nowFav = true;
            }
            else
            {
                _context.Favourites.Remove(fav);
                nowFav = false;
            }
            _context.SaveChanges();

            return Json(new { isFavorite = nowFav });
        }
    }
}