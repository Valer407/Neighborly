using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neighborly.Data;
using Neighborly.Models;
using Neighborly.Models.DBModels;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Neighborly.Controllers
{
    [Route("profil")]
    public class ProfileController : Controller
    {
        private readonly NeighborlyContext _context;

        public ProfileController(NeighborlyContext context)
        {
            _context = context;
        }

        [HttpGet("/profil/{id:int?}")]
        public IActionResult Index(int? id)
        {
            if (!id.HasValue)
            {
                var sessionId = HttpContext.Session.GetInt32("UserId");
                if (sessionId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                id = sessionId.Value;
            }

            var user = _context.Users
                .Include(u => u.City)
                .Include(u => u.District)
                .FirstOrDefault(u => u.UserId == id.Value && u.IsActive);
            if (user == null)
            {
                return NotFound();
            }

            var listings = _context.Listings
                .Where(l => l.UserId == user.UserId)
                .Include(l => l.City)
                .Include(l => l.District)
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
                        District = l.District.Name,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude
                    },
                    User = new ListingCardUserViewModel
                    {
                        Id = user.UserId,
                        Name = user.FirstName + " " + user.LastName,
                        Avatar = user.AvatarUrl,
                        Rating = user.RatingAvg
                    }
                })
                .ToList();

                       var reviews = (from r in _context.User_Ratings
                            where r.RateeId == user.UserId
                            join ru in _context.Users on r.RaterId equals ru.UserId
                            join c in _context.Categories on r.CategoryId equals c.CategoryId into cat
                            from c in cat.DefaultIfEmpty()
                            select new ProfileReviewViewModel
                            {
                                Author = new ReviewAuthorViewModel
                                {
                                    Name = ru.FirstName + " " + ru.LastName,
                                    Avatar = ru.AvatarUrl
                                },
                                Rating = r.Score,
                                Comment = r.Comment,
                                CategoryName = c != null ? c.Name : null,
                                Date = r.CreatedAt
                            })
                .OrderByDescending(r => r.Date)
                .ToList();

            var viewModel = new ProfileViewModel
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
                    City = user.City != null ? user.City.Name : string.Empty,
                    District = user.District != null ? user.District.Name : string.Empty
                },
                Listings = listings,
                Reviews = reviews
            };

            return View(viewModel);
        }
    }
}