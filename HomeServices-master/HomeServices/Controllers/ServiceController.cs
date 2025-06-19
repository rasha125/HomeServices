using HomeServices.Models;
using HomeServices.Models.Repositorie;
using HomeServices.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Controllers
{
    public class ServiceController : Controller
    {

        IRepositorie<Services,int> _rep;
        IRepositorie<Providers, int> _providerRep;
        IRepositorie<Ratings, int> _ratingRep;


        public ServiceController(IRepositorie<Services, int> rep, IRepositorie<Providers, int> providerRep , IRepositorie<Ratings, int> ratingRep)

        {
            _rep = rep;
            _providerRep = providerRep;
            _ratingRep = ratingRep;

        }

        public ActionResult Create(Services collection)
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
                _rep.Update(id , collection);
                return RedirectToAction(nameof(Index));
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
                _rep.Delete(id , collection);
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

        public ActionResult Index()
        {
            var services = _rep.View().ToList();
            return View(services);
        }

        public ActionResult Providers(int serviceId)
        {
            var service = _rep.Find(serviceId);
            if (service == null)
                return NotFound();

            var providers = _providerRep.View().Where(p => p.ServicesId == serviceId).ToList();

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
                ProvidersWithRatings = providersWithRatings
            };

            return View(model);
          
        }

    }
}
