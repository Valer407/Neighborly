using Microsoft.AspNetCore.Mvc;
using Neighborly.Models;
namespace Neighborly.Controllers
{
    public class AdminController : Controller
    {
        // Panel administratora
        public IActionResult Index()
        {
            // TODO: Pobierz wszystkie ogłoszenia z bazy danych
            /*var sampleListings = new List<AdminListingViewModel>
            {
                new AdminListingViewModel
                {
                    Id = 1,
                    Title = "Zakupy",
                    Author = "Jan Kowalski",
                    Category = "Zakupy",
                    Status = "Aktywne",
                    Date = DateTime.Parse("2024-01-14")
                },
                new AdminListingViewModel
                {
                    Id = 2,
                    Title = "Wyprowadzenie psa",
                    Author = "Anna Nowak",
                    Category = "Zwierzęta",
                    Status = "Aktywne",
                    Date = DateTime.Parse("2024-01-10")
                },
                new AdminListingViewModel
                {
                    Id = 3,
                    Title = "Naprawa kranu",
                    Author = "Piotr Wiśniewski",
                    Category = "Naprawy",
                    Status = "Zablokowane",
                    Date = DateTime.Parse("2024-01-03")
                }
            };

            // TODO: Pobierz wszystkich użytkowników z bazy danych
            // var users = dbContext.Users.ToList();

            // TODO: Pobierz wszystkie kategorie z bazy danych
            // var categories = dbContext.Categories.ToList();

            // TODO: Można przekazać wszystko do widoku np. przez ViewModel z trzema listami

            return View(sampleListings);*/
            return View(); // docelowo return View(listings);
        }

        // Usuwanie ogłoszenia
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // TODO: Usuń ogłoszenie z bazy danych na podstawie id
            return RedirectToAction("Index");
        }

        // TODO: Akcja do pobrania użytkowników (jeśli osobna strona/podstrona)
        public IActionResult Users()
        {
            // TODO: Pobierz użytkowników z bazy danych
            return View(); // docelowo return View(users);
        }

        // TODO: Akcja do zarządzania kategoriami
        public IActionResult Categories()
        {
            // TODO: Pobierz kategorie z bazy danych
            return View(); // docelowo return View(categories);
        }
        public IActionResult AddCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AddCategoryModal", model);

            // TODO: Zapisz kategorię do bazy danych

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetAddCategoryModal()
        {
            return PartialView("_AddCategoryModal", new CategoryViewModel());
        }
    }
}
