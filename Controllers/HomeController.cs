using Microsoft.AspNetCore.Mvc;

namespace Fruitables_MVC.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
            return View();
        }
    }
}
