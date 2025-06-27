using HomeServices.Models;
using HomeServices.Models.Repositorie;
using HomeServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Controllers
{
    public class ServiceController : Controller
    {

        IRepositorie<Services, int> _rep;
        IRepositorie<Providers, int> _providerRep;
        IRepositorie<Ratings, int> _ratingRep;
        private readonly UserManager<Users> _userManager;
        private readonly AppDBContext _context;


        public ServiceController(IRepositorie<Services, int> rep, IRepositorie<Providers, int> providerRep,
            IRepositorie<Ratings, int> ratingRep, UserManager<Users> userManager, AppDBContext context)

        {
            _rep = rep;
            _providerRep = providerRep;
            _ratingRep = ratingRep;
            _userManager = userManager;
            _context = context;
        }

        public ActionResult Create(Services collection)
        {
            try
            {
                _rep.Add(collection);
                return RedirectToAction(nameof(IndexServices));
            }
            catch
            {
                return View();
            }
        }

        // GET: ServicesController/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
        // POST: ServicesController/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Services collection)
        {
            try
            {
                _rep.Update(id, collection);
                return RedirectToAction(nameof(IndexServices));
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var service = _rep.Find(id);
                if (service == null)
                    return NotFound();

                _rep.Delete(id, service);
                return RedirectToAction(nameof(IndexServices));
            }
            catch
            {
                return View();
            }
        }



        // GET: ServicesController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        // POST: ServicesController/Delete/5
        public ActionResult Delete(int id, Services collection)
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

        // GET: ServicesController/Details/5
        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        [Authorize(Roles = "Client")]
        public ActionResult Index()
        {
            var services = _rep.View().ToList();
            return View(services);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexServices()
        {
            var services = _rep.View().ToList();
            return View(services);
        }


        [Authorize(Roles = "Client")]
        
        public ActionResult Providers(int serviceId)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var person = _context.Persons.FirstOrDefault(p => p.UserId == userId);

            if (person == null)
            {
                return BadRequest("Person not found for this user.");
            }

            var service = _rep.Find(serviceId);
            if (service == null)
                return NotFound();

            var personCountry = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Country)
                .FirstOrDefault();

            var providers = _providerRep.View()
                .Where(p => p.ServicesId == serviceId && p.User.Country == personCountry)
                .ToList();

            var ratings = _ratingRep.View()
                .Where(r => r.Orders.Providers.Services.ServicesId == serviceId)
                .ToList();

            var providersWithRatings = providers.Select(p => new ProviderWithRatingViewModel
            {
                Provider = p,
                AverageRating = ratings
                    .Where(r => r.Orders.Providers.ProvidersId == p.ProvidersId)
                    .Select(r => r.RatingValue)
                    .DefaultIfEmpty(0)
                    .Average()
            }).ToList();

            var model = new ServiceProvidersViewModel
            {
                ServiceName = service.ServiceName,
                ServiceId = service.ServicesId,
                ProvidersWithRatings = providersWithRatings,
                PersonId = person.PersonsId
            };

            return View(model);
        }
    }
    }
