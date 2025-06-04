using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Controllers
{
    public class ProviderController : Controller
    {
        IRepositorie<Providers> _rep;

        public ProviderController(IRepositorie<Providers> rep)
        {
            _rep = rep;
        }

        public ActionResult Create(Providers collection)
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
            var data = _rep.View().ToList();
            return View(data);
        }

        // GET: ProvidersController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
        // POST: ProvidersController/Edit/5
        public ActionResult Edit(int id, Providers collection)
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

        // GET: ProvidersController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        // POST: ProvidersController/Delete/5
        public ActionResult Delete(int id, Providers collection)
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

        // GET: ProvidersController/Details/5
        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
    }
}
