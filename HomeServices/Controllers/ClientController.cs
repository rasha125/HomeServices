using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Controllers
{
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
