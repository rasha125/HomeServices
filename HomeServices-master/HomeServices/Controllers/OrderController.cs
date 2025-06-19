using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Controllers
{
    public class OrderController : Controller
    {

        IRepositorie<Orders,int> _rep;



        public OrderController(IRepositorie<Orders, int> rep)

        {
            _rep = rep;
        }

        public ActionResult Create(Orders collection)
        {
            try
            {
                _rep.Add(collection);
                return  RedirectToAction("Orders", "Person");
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

        // GET: OrdersController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
        // POST: OrdersController/Edit/5
        public ActionResult Edit(int id, Orders collection)
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

        // GET: OrdersController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        // POST: OrdersController/Delete/5
        public ActionResult Delete(int id, Orders collection)
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

        // GET: OrdersController/Details/5
        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
    }
}
