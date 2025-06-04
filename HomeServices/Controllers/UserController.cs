using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeUsers.Controllers
{
    public class UserController : Controller
    {
        IRepositorie<Users> _rep;

        public UserController(IRepositorie<Users> rep)
        {
            _rep = rep;
        }

        public ActionResult Create(Users collection)
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

        // GET: UsersController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
        // POST: UsersController/Edit/5
        public ActionResult Edit(int id, Users collection)
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

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        // POST: UsersController/Delete/5
        public ActionResult Delete(int id, Users collection)
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

        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
    }
}
