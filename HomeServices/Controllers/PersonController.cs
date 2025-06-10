using HomeServices.Models;
using HomeServices.Models.Repositorie;
using HomeServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace HomeServices.Controllers
{
    [Authorize(Roles = "Client")]
    public class PersonController : Controller
    {
        IRepositorie<Persons, int> _rep;
        IRepositorie<Providers, int> _providerRep;
        IHttpContextAccessor _httpContextAccessor;

        public PersonController(
            IRepositorie<Persons, int> rep,
            IRepositorie<Providers, int> providerRep,
            IHttpContextAccessor httpContextAccessor)
        {
            _rep = rep;
            _providerRep = providerRep;
            _httpContextAccessor = httpContextAccessor;
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

        public IActionResult Index()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            var currentPerson = _rep.View().FirstOrDefault(p => p.User.UserName == userName);

            var people = _rep.View().ToList();

            var nearbyProviders = currentPerson != null
                ? _providerRep.View().Where(p => p.User.City == currentPerson.User.City).ToList()
                : new System.Collections.Generic.List<Providers>();

            var viewModel = new VMPersonIndex
            {
                People = people,
                NearbyProviders = nearbyProviders
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
