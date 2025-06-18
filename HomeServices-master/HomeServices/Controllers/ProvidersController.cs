using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeServices.ViewModels;

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

        public IActionResult dashboard()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            var currentProvider = _rep.View()
                .FirstOrDefault(p => p.User.UserName == userName);

            if (currentProvider == null)
                return Unauthorized();

            var viewModel = new VMProviderProfile
            {
                FirstName = currentProvider.User.FirstName,
                LastName = currentProvider.User.LastName,
            };

            return View(viewModel);
        }

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
                ImagePath = user.ImagePath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(VMProviderProfile model, IFormFile? ProfileImage, string? actionType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var provider = _context.Providers.FirstOrDefault(p => p.UserId == user.Id);
            if (provider == null) return NotFound();

            if (actionType == "delete")
            {
                // حذف الصورة القديمة من السيرفر لو موجودة
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", user.ImagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    user.ImagePath = null;
                }
                // لا ترجع هنا، استمر بتحديث باقي البيانات
            }

            // لو جرب يرفع صورة جديدة (حتى لو كانت delete، يتجاهل لأنها لن تكون null إذا رفع)
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // حذف الصورة القديمة لو موجودة
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

            // تحديث بيانات المستخدم من النموذج
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.City = model.City;
            user.Country = model.Country;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "User data update failed.");
                return View(model);
            }

            // تحديث بيانات الموفر
            provider.Description = model.Description;
            provider.UpdatedAt = DateTime.UtcNow;

            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();

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
                _rep.Delete(id, collection);
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

