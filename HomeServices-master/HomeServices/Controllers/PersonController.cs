using HomeServices.Models;
using HomeServices.Models.Repositorie;
using HomeServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace HomeServices.Controllers
{
    //[Authorize(Roles = "Client")]
    public class PersonController : Controller
    {
        private readonly IRepositorie<Persons, int> _rep;
        private readonly IRepositorie<Providers, int> _providerRep;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Users> _userManager;
        private readonly AppDBContext _context;

        public PersonController(
            IRepositorie<Persons, int> rep,
            IRepositorie<Providers, int> providerRep,
            IHttpContextAccessor httpContextAccessor,
            UserManager<Users> userManager,
            AppDBContext context)
        {
            _rep = rep;
            _providerRep = providerRep;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
        }

        public ActionResult Create(Persons collection)
        {
            try
            {
                _rep.Add(collection);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var data = _rep.View().ToList();
            return View(data);
        }

        [Authorize(Roles = "Client")]
        public IActionResult dashboard()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            var currentPerson = _rep.View().FirstOrDefault(p => p.User.UserName == userName);

            var viewModel = new VMPersonIndex
            {
                FirstName = currentPerson.User.FirstName,
                LastName = currentPerson.User.LastName,
            };


            return View(viewModel);
        }
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
     


        [HttpPost]
        public ActionResult Edit(int id, Persons collection)
        {
            try
            {
                _rep.Update(id, collection);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult Delete(int id, Persons collection)
        {
            try
            {
                _rep.Delete(id, collection);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
    }
}
