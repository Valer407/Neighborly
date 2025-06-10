using Microsoft.AspNetCore.Mvc;

namespace Neighborly.Controllers
{
    public class AccountController : Controller
    {
        //private readonly ApplicationDbContext _context;

        // to do 
        // public AccountController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // TODO: sprawdzenie użytkownika w bazie danych
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email i hasło są wymagane.");
                return View();
            }

            // TODO: znajdź użytkownika po email i sprawdź poprawność hasła
            bool isValidUser = false; // <- tu sprawdzanie z bazy

            if (!isValidUser)
            {
                ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
                return View();
            }

            // TODO: ustaw cookie / sesję / logowanie
            return RedirectToAction("Index", "Home");
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

            // TODO: sprawdzenie w bazie czy użytkownik już istnieje
            //var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            //if (existingUser != null)
                //ModelState.AddModelError("email", "Użytkownik z takim adresem email już istnieje.");

            if (!ModelState.IsValid)
                return View();

            // TODO: zapis nowego użytkownika do bazy

            return RedirectToAction("Login");
        }
    }
}
