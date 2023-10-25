using Microsoft.AspNetCore.Mvc;

namespace GOPH.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
