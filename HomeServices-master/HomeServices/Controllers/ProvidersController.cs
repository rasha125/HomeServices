using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeServices.ViewModels;
using HomeServices.Enums;

namespace HomeServices.Controllers
{
    
    public class ProviderController : Controller
    {

        IRepositorie<Providers, int> _rep;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Users> _userManager;
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProviderController(
            IRepositorie<Providers, int> rep,
            IHttpContextAccessor httpContextAccessor,
            UserManager<Users> userManager,
            AppDBContext context,
            IWebHostEnvironment environment)
        {
            _rep = rep;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
            _environment = environment;
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
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var data = _rep.View().ToList();
            return View(data);
        }

        [Authorize(Roles = "Provider")]
        public IActionResult Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            var provider = _context.Providers
                .FirstOrDefault(p => p.UserId == userId);

            if (provider == null)
                return Unauthorized();

            // جلب كل الطلبات الخاصة بالموفر (بما فيهم جميع الحالات)
            var orders = _context.Orders
                .Where(o => o.ProviderId == provider.ProvidersId)
                .ToList();


            var totalCount = orders.Count();
            var completedCount = orders.Count(o => o.Status == OrderStatus.Completed.ToString());
            var cancelledCount = orders.Count(o => o.Status == OrderStatus.Cancelled.ToString());
            var pendingCount = totalCount - completedCount - cancelledCount;


            var currentDate = DateTime.Now;
            var recentOrders = _context.Orders
                .Include(o => o.Persons).ThenInclude(p => p.User)
                .Include(o => o.Providers).ThenInclude(p => p.Services)
                .Where(o => o.ProviderId == provider.ProvidersId &&
                            o.OrdersDate.Month == currentDate.Month &&
                            o.OrdersDate.Year == currentDate.Year)
                .OrderByDescending(o => o.OrdersDate)
                .ToList();


            var viewModel = new VMProviderDashboard
            {
                PendingCount = pendingCount,
                CompletedCount = completedCount,
                TotalCount = totalCount,
            };
            ViewBag.RecentOrders = recentOrders;
            return View(viewModel);
        }

        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var provider = _context.Providers
                .Include(p => p.Services)
                .FirstOrDefault(p => p.UserId == user.Id);

            if (provider == null) return NotFound();

            var model = new VMProviderProfile
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber,
                Description = provider.Description,
                ProviderStatus = provider.ProviderStatus,
                ServiceName = provider.Services?.ServiceName,
                ImagePath = user.ImagePath
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> UploadProfileImage(IFormFile ProfileImage)
        {
            // التأكد من المستخدم
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (ProfileImage == null || ProfileImage.Length == 0)
            {
                TempData["Error"] = "No image selected.";
                return RedirectToAction("Profile");
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // حذف الصورة القديمة إذا كانت موجودة
            if (!string.IsNullOrEmpty(user.ImagePath))
            {
                var oldPath = Path.Combine(uploadsFolder, user.ImagePath);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            // إنشاء اسم فريد للصورة
            var fileExt = Path.GetExtension(ProfileImage.FileName);
            var uniqueFileName = $"{user.Id}_{Guid.NewGuid()}{fileExt}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // حفظ الصورة
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(fileStream);
            }

            // تحديث بيانات المستخدم
            user.ImagePath = uniqueFileName;
            await _userManager.UpdateAsync(user);


            return RedirectToAction("Profile");
        }

        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var provider = _context.Providers.FirstOrDefault(p => p.UserId == user.Id);
            if (provider == null) return NotFound();

            var model = new VMProviderProfile
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber,
                Description = provider.Description,
                ProviderStatus = provider.ProviderStatus,
                ImagePath = user.ImagePath
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> EditProfile(VMProviderProfile model, IFormFile? ProfileImage, string? actionType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var provider = _context.Providers.FirstOrDefault(p => p.UserId == user.Id);
            if (provider == null) return NotFound();

            
            if (actionType == "delete")
            {
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", user.ImagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    user.ImagePath = null;
                }
            }


            if ((actionType == "change" || string.IsNullOrEmpty(actionType)) && ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    var oldFile = Path.Combine(uploadsFolder, user.ImagePath);
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                user.ImagePath = uniqueFileName;
            }



            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.City = model.City;
            user.Country = model.Country;
            user.PhoneNumber = model.PhoneNumber;

            var userResult = await _userManager.UpdateAsync(user);
            if (!userResult.Succeeded)
            {
                ModelState.AddModelError("", "User data update failed.");
                return View(model);
            }

          
            if (!string.IsNullOrEmpty(model.Description))
                provider.Description = model.Description;


            if (!string.IsNullOrEmpty(model.ProviderStatus))
                provider.ProviderStatus = model.ProviderStatus;

            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }





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
                _rep.Update(id, collection);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var provider = _context.Providers.FirstOrDefault(p => p.ProvidersId == id);

            if (provider == null)
            {
                return NotFound();
            }

            provider.DeletedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        

        // GET: ProvidersController/Details/5
        public ActionResult Details(int id)
        {
            var provider = _context.Providers
      .Include(p => p.User)
      .Include(p => p.Services)
      .FirstOrDefault(p => p.ProvidersId == id);

            if (provider == null)
            {
                return NotFound();
            }

            return View(provider);
         
        }




        public ActionResult ListOfPersons()
        {
            var currentUserId = _userManager.GetUserId(User);

            var lastMessages = _context.Messages
                .Where(m => m.ReceiverId == currentUserId || m.SenderId == currentUserId)
                .OrderByDescending(m => m.SentAt)
                .ToList()
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => g.First())
                .ToList();

            int newMessagesCount = lastMessages.Count(m => m.SenderId != currentUserId);

            ViewBag.NewMessagesCount = newMessagesCount;




            var Persons = _context.Persons.Include(p => p.User).ToList();
            return View(Persons);
        }
    }


}

