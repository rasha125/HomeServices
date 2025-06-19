using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Controllers
{
    public class ServiceController : Controller
    {

        IRepositorie<Services,int> _rep;


        public ServiceController(IRepositorie<Services, int> rep)

        {
            _rep = rep;
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
            // show all providers for the service (to be implemented)
            return View();
        }
    }
}
