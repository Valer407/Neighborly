using Microsoft.AspNetCore.Mvc;
using Neighborly.Models;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Globalization;
using System.Text.Json;

namespace Neighborly.Controllers
{
    public class ListingsController : Controller
    {
        private readonly NeighborlyContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public ListingsController(NeighborlyContext context, IWebHostEnvironment env, IConfiguration config)
        {
            _context = context;
            _env = env;
            _config = config;
        }
        public IActionResult Index(string search, string type)
        {
            var categories = _context.Categories.ToList();
            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.IconSvg))
                {
                    category.IconSvg = Icons.GetIcon(category.Icon);
                }
            }

            var listingsQuery = _context.Listings
                .Include(l => l.City)
                .Include(l => l.District)
                .Include(l => l.User)
                .Include(l => l.ListingType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                listingsQuery = listingsQuery.Where(l => l.Title.Contains(search) || l.Description.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (type == "offer")
                {
                    listingsQuery = listingsQuery.Where(l => l.ListingType.Name == "Oferuję pomoc");
                }
                else if (type == "request")
                {
                    listingsQuery = listingsQuery.Where(l => l.ListingType.Name == "Szukam pomocy");
                }
            }

            var listings = listingsQuery
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
                        District = l.District.Name,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude
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
          ViewBag.SelectedType = type;

            var viewModel = new ListingsIndexViewModel
            {
                Categories = categories,
                Listings = listings,
                SearchQuery = search
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
                        Id = listing.User.UserId,
                        Name = listing.User.FirstName + " " + listing.User.LastName,
                        Avatar = listing.User.AvatarUrl,
                        Rating = listing.User.RatingAvg
                },
                    Latitude = listing.Latitude,
                    Longitude = listing.Longitude
                },
                IsFavorite = isFav
            };
            ViewBag.GoogleApiKey = _config["GoogleMaps:ApiKey"];
            return View(viewModel);
        }

        // GET: /Listings/NewListing
        [HttpGet]
        public IActionResult NewListing()
        {
            ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.ListingTypes = _context.Listing_Types.OrderBy(t => t.Name).ToList();
            ViewBag.GoogleApiKey = _config["GoogleMaps:ApiKey"];
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

            if (listing.Latitude == 0 && listing.Longitude == 0)
            {
                var coords = GetCoordinates(districtEntity.Name, cityEntity.Name);
                listing.Latitude = coords.lat;
                listing.Longitude = coords.lon;
            }
            listing.CreatedAt = DateTime.UtcNow;
            listing.UpdatedAt = DateTime.UtcNow;

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

        [HttpGet]
        [Route("edytuj-ogloszenie/{id}")]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var listing = _context.Listings
                .Include(l => l.City)
                .Include(l => l.District)
                .FirstOrDefault(l => l.ListingId == id && l.UserId == userId.Value);

            if (listing == null)
            {
                return RedirectToAction("MyAccount", "Account");
            }

            ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.ListingTypes = _context.Listing_Types.OrderBy(t => t.Name).ToList();
            ViewBag.GoogleApiKey = _config["GoogleMaps:ApiKey"];
            return View("Edit", listing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edytuj-ogloszenie/{id}")]
        public IActionResult Edit(int id, Listings listing, string City, string District, List<IFormFile> images)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existing = _context.Listings.FirstOrDefault(l => l.ListingId == id && l.UserId == userId.Value);
            if (existing == null)
            {
                return RedirectToAction("MyAccount", "Account");
            }

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
                return View("Edit", listing);
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
                return View("Edit", listing);
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

            existing.Title = listing.Title;
            existing.Description = listing.Description;
            existing.Price = listing.Price;
            existing.CategoryId = listing.CategoryId;
            existing.ListingTypeId = listing.ListingTypeId;
            existing.CityId = cityEntity.CityId;
            existing.DistrictId = districtEntity.DistrictId;
            if (listing.Latitude == 0 && listing.Longitude == 0)
            {
                var coordsUpdate = GetCoordinates(districtEntity.Name, cityEntity.Name);
                existing.Latitude = coordsUpdate.lat;
                existing.Longitude = coordsUpdate.lon;
            }
            else
            {
                existing.Latitude = listing.Latitude;
                existing.Longitude = listing.Longitude;
            }
            existing.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            if (images != null && images.Count > 0)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "listings");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var existingImages = _context.Listing_Images
                    .Where(i => i.ListingId == existing.ListingId)
                    .ToList();

                foreach (var img in existingImages)
                {
                    var fullPath = Path.Combine(_env.WebRootPath, img.Url.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    _context.Listing_Images.Remove(img);
                }
                _context.SaveChanges();

                int order = 0;
                foreach (var file in images)
                {
                    if (file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png") continue;
                    if (file.Length > 5 * 1024 * 1024) continue;

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var imgEntity = new Listing_images
                    {
                        ListingId = existing.ListingId,
                        Url = $"/uploads/listings/{fileName}",
                        SortOrder = order++
                    };
                    _context.Listing_Images.Add(imgEntity);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("MyAccount", "Account");
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
        public (float lat, float lon) GetCoordinates(string district, string city)
        {
            using var httpClient = new HttpClient();
            var apiKey = _config["GoogleMaps:ApiKey"];
            var address = $"{district}, {city}, Poland";
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";
            var response = httpClient.GetStringAsync(url).GetAwaiter().GetResult();
            using var json = JsonDocument.Parse(response);
            var first = json.RootElement.GetProperty("results").EnumerateArray().FirstOrDefault();

            if (first.ValueKind != JsonValueKind.Undefined)
            {
                var location = first.GetProperty("geometry").GetProperty("location");
                float lat = location.GetProperty("lat").GetSingle();
                float lon = location.GetProperty("lng").GetSingle();
                return (lat, lon);
            }

            throw new Exception("Nie znaleziono lokalizacji w Google");
        }
    }
}