using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
