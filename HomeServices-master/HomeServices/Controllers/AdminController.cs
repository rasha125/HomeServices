using HomeServices.Models.Repositorie;
using HomeServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace HomeServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDBContext _context;

        public AdminController(AppDBContext context)
        {
            _context = context;
        }


        public IActionResult Dashboard()
        {
            int userCount = _context.Users.Count(u => u.DeletedAt == null)-1;
            int personCount = _context.Persons.Count(p => p.DeletedAt == null);
            int providerCount = _context.Providers.Count(p => p.DeletedAt == null);
            int orderCount = _context.Orders.Count(); 
            int issueCount = _context.Issues.Count();

            ViewBag.UserCount = userCount;
            ViewBag.PersonCount = personCount;
            ViewBag.ProviderCount = providerCount;
            ViewBag.orderCount = orderCount;
            ViewBag.issueCount = issueCount;

            return View();
        }

    }
}
