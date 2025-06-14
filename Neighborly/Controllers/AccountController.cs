using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Neighborly.Models;

namespace Neighborly.Controllers
{

    public class AccountController : Controller
    {
        private readonly NeighborlyContext _context;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public AccountController(NeighborlyContext context)
        {
            _context = context;
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
            return RedirectToAction("Login");
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