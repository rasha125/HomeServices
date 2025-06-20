using HomeServices.Enums;
using HomeServices.Models;
using HomeServices.Models.Repositorie;
using HomeServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Controllers
{
    public class IssueController : Controller
    {

        IRepositorie<Issues, int> _rep;
        private readonly AppDBContext _context;
        private readonly UserManager<Users> _userManager;


        public IssueController(
            IRepositorie<Issues, int> rep,
            AppDBContext context,
            UserManager<Users> userManager)
        {
            _rep = rep;
            _context = context;
            _userManager = userManager;
        }

        public ActionResult Create(Issues collection)
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
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var data = _rep.View().ToList();
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProviderIssue(VMIssue model, IFormFile uploadedFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);
            string uniqueFileName = null;

            // حفظ الملف في مجلد wwwroot/uploads
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
            }

            // إنشاء البلاغ
            var issue = new Issues
            {
                Type = model.Type,
                Description = model.Description,
                UserId = userId,
                File = uniqueFileName, // اسم الملف إذا تم رفعه
                Status = ReportStatus.InProgress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Issue submitted successfully.";
            return RedirectToAction("ProviderIssue");
        }


        // GET: IssuesController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }
        // POST: IssuesController/Edit/5
        public ActionResult Edit(int id, Issues collection)
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

        // GET: IssuesController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        // POST: IssuesController/Delete/5
        public ActionResult Delete(int id, Issues collection)
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

        [Authorize(Roles = "Admin")]
        // GET: IssuesController/Details/5
        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeStatus(int id)
        {
            var issue = _context.Issues.Find(id);
            if (issue != null && issue.Status != ReportStatus.Closed)
            {
                issue.Status = ReportStatus.Closed;
                issue.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }





        [HttpGet]
        [Authorize(Roles = "Provider,Client")]
        public IActionResult SentIssue()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Provider,Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SentIssue(VMIssue model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // الحصول على المستخدم الحالي
            var userId = _userManager.GetUserId(User);

            // إنشاء البلاغ
            var issue = new Issues
            {
                Type = model.Type,
                Description = model.Description,
                UserId = userId,
                Status = ReportStatus.InProgress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Issue submitted successfully.";
            if (User.IsInRole("Client"))
            {
                return RedirectToAction("Dashboard", "Person"); // أو الصفحة المناسبة للعميل
            }
            else if (User.IsInRole("Provider"))
            {
                return RedirectToAction("Dashboard", "Provider"); // أو أي صفحة رئيسية للمزود
            }


            return RedirectToAction("Index", "Home");


        }



    }
}
