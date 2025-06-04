using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Controllers
{
    public class PersonController : Controller
    {
        IRepositorie<Persons> _rep;

        public PersonController(IRepositorie<Persons> rep)
        {
            _rep = rep;
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
            var data = _rep.View().ToList();
            return View(data);
        }

        // GET: PersonsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
        // POST: PersonsController/Edit/5
        public ActionResult Edit(int id, Persons collection)
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

        // GET: PersonsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        // POST: PersonsController/Delete/5
        public ActionResult Delete(int id, Persons collection)
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

        // GET: PersonsController/Details/5
        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
    }
}
