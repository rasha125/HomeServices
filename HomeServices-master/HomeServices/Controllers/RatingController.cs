using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Controllers
{
    public class RatingController : Controller
    {

        IRepositorie<Ratings,int> _rep;



        public RatingController(IRepositorie<Ratings, int> rep)

        {
            _rep = rep;
        }

        public ActionResult Create(Ratings collection)
        {
            try
            {
                _rep.Add(collection);
                return RedirectToAction("Orders","Person");
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

        // GET: RatingsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
        // POST: RatingsController/Edit/5
        public ActionResult Edit(int id, Ratings collection)
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

        // GET: RatingsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        // POST: RatingsController/Delete/5
        public ActionResult Delete(int id, Ratings collection)
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

        // GET: RatingsController/Details/5
        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
    }
}
