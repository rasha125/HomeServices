using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Controllers
{
    public class RatingController : Controller
    {

        IRepositorie<Ratings, int> _rep;
        private readonly AppDBContext _context;



        public RatingController(IRepositorie<Ratings, int> rep, AppDBContext context)

        {
            _rep = rep;
            _context = context;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public IActionResult Create(Ratings rating)
        {
            try
            {
                // عبي تاريخ الإنشاء والتحديث
                rating.CreatedAt = DateTime.UtcNow;
                rating.UpdatedAt = DateTime.UtcNow;

                // تأكد أن قيم OrdersId و PersonsId موجودة في rating من الفورم

                if (rating.OrdersId == 0 || rating.PersonsId == 0)
                {
                    ModelState.AddModelError("", "Order or Person information is missing.");
                    return View("Error"); // أو معالجة مناسبة
                }

                // أضف التقييم إلى الريبو
                _rep.Add(rating);

                // إعادة توجيه مثلاً إلى صفحة الطلبات أو أي مكان آخر
                return RedirectToAction("Orders", "Person");
            }
            catch (Exception ex)
            {
                // في حال حدوث خطأ
                ModelState.AddModelError("", "Failed to save rating: " + ex.Message);
                return View("Error");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitRating(int OrdersId, int PersonsId, int RatingValue, string? Comment)
        {
            if (OrdersId == 0 || PersonsId == 0)
            {
                return BadRequest("User or Order information is missing. Please try again.");
            }
            if (RatingValue < 1 || RatingValue > 5)
            {
                return BadRequest("Invalid rating value. Please select between 1 and 5 stars.");
            }

            var rating = new Ratings
            {
                OrdersId = OrdersId,
                PersonsId = PersonsId,
                RatingValue = RatingValue,
                Comment = Comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _rep.Add(rating);

            // بدلاً من إعادة التوجيه، أرسل استجابة نجاح
            return Ok("Rating submitted successfully!");
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
                _rep.Update(id, collection);
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
                _rep.Delete(id, collection);
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
