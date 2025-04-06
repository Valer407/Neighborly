using Microsoft.AspNetCore.Mvc;

namespace Neighborly.Controllers
{
    public class ListingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}