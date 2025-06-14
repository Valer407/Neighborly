using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Neighborly.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Neighborly.Controllers
{

    public class AccountController : Controller
    {
        private readonly NeighborlyContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public AccountController(NeighborlyContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult MyAccount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login");

            var listings = _context.Listings
                .Where(l => l.UserId == userId.Value)
                .Include(l => l.City)
                .Include(l => l.District)
                .Include(l => l.ListingType)
                .Select(l => new ListingViewModel
                {
                    Id = l.ListingId,
                    Title = l.Title,
                    Description = l.Description,
                    CreatedAt = l.CreatedAt,
                    Type = l.ListingType.Name == "Oferuję pomoc" ? "offer" : "request",
                    Location = new LocationViewModel
                    {
                        City = l.City.Name,
                        District = l.District.Name
                    }
                })
                .ToList();
            
             var favorites = _context.Favourites
                .Where(f => f.UserId == userId.Value)
                .Include(f => f.Listing)
                    .ThenInclude(l => l.City)
                .Include(f => f.Listing)
                    .ThenInclude(l => l.District)
                .Include(f => f.Listing)
                    .ThenInclude(l => l.ListingType)
                .Select(f => new ListingViewModel
                {
                    Id = f.Listing.ListingId,
                    Title = f.Listing.Title,
                    Description = f.Listing.Description,
                    CreatedAt = f.Listing.CreatedAt,
                    Type = f.Listing.ListingType.Name == "Oferuję pomoc" ? "offer" : "request",
                    Location = new LocationViewModel
                    {
                        City = f.Listing.City.Name,
                        District = f.Listing.District.Name
                    }
                })
                .ToList();

            var model = new MyAccountViewModel
            {
                Listings = listings,
                Favorites = favorites
            };
            ViewBag.UserId = userId.Value;
            return View(model);
        }
        
// GET: /Account/EditProfile
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
            if (user == null)
                return RedirectToAction("Login");

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AvatarUrl = string.IsNullOrEmpty(user.AvatarUrl) ? "/assets/default-avatar.png" : user.AvatarUrl,
                City = user.City,
                District = user.District,
                About = user.About
            };

            return View(model);
        }

        // POST: /Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileViewModel model, IFormFile? avatar)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
            if (user == null)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Email = model.Email ?? user.Email;
            user.City = model.City;
            user.District = model.District;
            user.About = model.About;

            if (avatar != null && avatar.Length > 0)
            {
                var ext = Path.GetExtension(avatar.FileName).ToLowerInvariant();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                {
                    if (avatar.Length <= 5 * 1024 * 1024)
                    {
                        var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        var fileName = $"{Guid.NewGuid()}{ext}";
                        var filePath = Path.Combine(uploadPath, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        avatar.CopyTo(stream);
                        user.AvatarUrl = $"/uploads/avatars/{fileName}";
                    }
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Profile");
        }

        // GET: /profil/{id?}
        [HttpGet]
        [Route("profil/{id?}")]
        public IActionResult Profile(int? id)
        {
            if (id == null)
            {
                id = HttpContext.Session.GetInt32("UserId");
                if (id == null)
                    return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == id.Value);
            if (user == null)
                return NotFound();

            var model = new ProfileViewModel
            {
                User = new ProfileUserViewModel
                {
                    Id = user.UserId,
                    Name = user.FirstName + " " + user.LastName,
                    Avatar = user.AvatarUrl,
                    Verified = false,
                    Rating = user.RatingAvg,
                    MemberSince = user.CreatedAt,
                    About = user.About ?? string.Empty,
                    City = user.City?.Name ?? string.Empty,
                    District = user.District?.Name ?? string.Empty
                }
            };

            return View(model);
        }
        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password, bool remember)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email i hasło są wymagane.");
                LoginUnsuccessful(email);
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.IsActive);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
                LoginUnsuccessful(email);
                return View();
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
                LoginUnsuccessful(email);
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            user.LastLogin = DateTime.UtcNow;
            _context.SaveChanges();

            LoginSuccessful(user);
            return Redirect("/");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string password, string confirmPassword, bool agreeTerms)
        {
            if (!agreeTerms)
                ModelState.AddModelError("agreeTerms", "Musisz zaakceptować regulamin.");

            if (password != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Hasła nie są zgodne.");

            if (_context.Users.Any(u => u.Email == email))
                ModelState.AddModelError("email", "Użytkownik z takim adresem email już istnieje.");

            if (!ModelState.IsValid)
                return View();

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                AvatarUrl = "/assets/default-avatar.png",
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, password);

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.UserId);

            LoginSuccessful(user);

            return Redirect("/");
        }
//logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
        }

        private void LoginSuccessful(User user)
        {
            // TODO: logic for successful login notification
        }

        private void LoginUnsuccessful(string email)
        {
            // TODO: logic for unsuccessful login notification
        }
    }
}