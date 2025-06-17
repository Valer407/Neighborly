using Microsoft.AspNetCore.Mvc;
using Neighborly.Models;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Microsoft.EntityFrameworkCore;
namespace Neighborly.Controllers
{
    public class AdminController : Controller
    {
        private readonly NeighborlyContext _context;

        public AdminController(NeighborlyContext context)
        {
            _context = context;
        }

        // Panel administratora
        public IActionResult Index()
        {
        var model = new AdminPanelViewModel
        {
                Listings = _context.Listings
                    .Include(l => l.User)
                    .Include(l => l.Category)
                    .Select(l => new AdminListingItem
                    {
                        Id = l.ListingId,
                        Title = l.Title,
                        Author = l.User.FirstName + " " + l.User.LastName,
                        Category = l.Category.Name,
                        Status = l.Status,
                        Date = l.CreatedAt
                    }).ToList(),
                Users = _context.Users
                    .Select(u => new AdminUserItem
                    {
                        Id = u.UserId,
                        Name = u.FirstName + " " + u.LastName,
                        Email = u.Email,
                        IsActive = u.IsActive,
                        Joined = u.CreatedAt
                    }).ToList(),
                Categories = _context.Categories.ToList(),
                Reports = _context.Reports
                    .Select(r => new AdminReportItem
                    {
                        Id = r.ReportId,
                        ListingTitle = _context.Listings.Where(l => l.ListingId == r.ListingId).Select(l => l.Title).FirstOrDefault(),
                        ReporterName = _context.Users.Where(u => u.UserId == r.ReporterId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                        Reason = r.Reason,
                        Status = r.Status,
                        Date = r.CreatedAt
                    }).ToList()
        };
            return View(model);
        }

        // Usuwanie ogłoszenia
        [HttpPost]
        public IActionResult DeleteListing(int id)
        {
            var listing = _context.Listings.Find(id);
            if (listing != null)
            {
                _context.Listings.Remove(listing);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteReport(int id)
        {
            var report = _context.Reports.Find(id);
            if (report != null)
            {
                _context.Reports.Remove(report);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult AddCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AddCategoryModal", model);

            var category = new Categories
            {
                Name = model.Name,
                IconSvg = model.IconSvg,
                Icon = model.Id
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetAddCategoryModal()
        {
            return PartialView("_AddCategoryModal", new CategoryViewModel());
        }
    }
}
