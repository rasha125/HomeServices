using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Controllers
{
    public class MessageController : Controller
    {
        IRepositorie<Messages> _rep;

        public MessageController(IRepositorie<Messages> rep)
        {
            _rep = rep;
        }

        public ActionResult Create(Messages collection)
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

        // GET: MessagesController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
        // POST: MessagesController/Edit/5
        public ActionResult Edit(int id, Messages collection)
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

        // GET: MessagesController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        // POST: MessagesController/Delete/5
        public ActionResult Delete(int id, Messages collection)
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

        // GET: MessagesController/Details/5
        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
    }
}
