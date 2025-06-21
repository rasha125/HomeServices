using HomeServices.Models;
using HomeServices.Models.Repositorie;
using HomeServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Security.Claims;



namespace HomeServices.Controllers
{
    //[Authorize(Roles = "Client")]
    public class PersonController : Controller
    {
        private readonly IRepositorie<Persons, int> _rep;
        private readonly IRepositorie<Providers, int> _providerRep;
        private readonly IRepositorie<Orders, int> _orderRep;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Users> _userManager;
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public PersonController(
            IRepositorie<Persons, int> rep,
            IRepositorie<Providers, int> providerRep,
            IRepositorie<Orders, int> orderRep,
            IHttpContextAccessor httpContextAccessor,
            UserManager<Users> userManager,
            AppDBContext context,
            IWebHostEnvironment environment)
        {
            _rep = rep;
            _providerRep = providerRep;
            _orderRep = orderRep;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
            _environment = environment;
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

        //[HttpPost]
        //[Authorize(Roles = "Client")]
        //public async Task<IActionResult> EditProfile(Persons model)
        //{
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    if (user == null) return NotFound();

        //    // تحديث البيانات
        //    user.FirstName = model.User.FirstName;
        //    user.LastName = model.User.LastName;
        //    user.PhoneNumber = model.User.PhoneNumber;
        //    //user.Email = model.User.Email;
        //    user.City = model.User.City;
        //    user.Country = model.User.Country;

        //    await _userManager.UpdateAsync(user);

        //    return RedirectToAction("Profile");
        //}


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var data = _rep.View().ToList();
            return View(data);
        }


        public ActionResult Edit(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }



        [HttpPost]
        public ActionResult Edit(int id, Persons collection)
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

        public ActionResult Delete(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult Delete(int id, Persons collection)
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

        public ActionResult Details(int id)
        {
            var data = _rep.Find(id);
            return View(data);
        }

        [Authorize(Roles = "Client")]
        public ActionResult Dashboard()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            // استرجاع بيانات الشخص الحالي
            var currentPerson = _rep.View().FirstOrDefault(p => p.User.UserName == userName);

            if (currentPerson == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // تصفية المزودين حسب المدينة
            var providers = _providerRep.View()
                .Where(p => p.User.City == currentPerson.User.City)
                .ToList();

            // استرجاع الأوردرات الخاصة بالمستخدم الحالي
            var orders = _orderRep.View()
                .Where(o => o.Persons.User.UserName == userName)
                .ToList();

            var model = new VMPersonIndex
            {
                PersonInfo = currentPerson,
                NearbyProviders = providers,
                MyOrders = orders
            };

            return View(model);
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Profile()
        {
            // ... (هذه الدالة صحيحة) ...
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (person == null) return NotFound();

            var model = new VMPersonProfile
            {
                PersonId = person.PersonsId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                City = user.City,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber,
                ImagePath = user.ImagePath
            };

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> EditProfile(VMPersonProfile model, IFormFile? ProfileImage, string? actionType)
        {
            // The first and most important check.
            if (!ModelState.IsValid)
            {
                // If the form data is invalid, return immediately with errors.
                // We also need to pass the current image path back so the view doesn't break.
                var currentUserForImage = await _userManager.GetUserAsync(User);
                model.ImagePath = currentUserForImage?.ImagePath;
                return View("Profile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // --- Image Handling Logic ---

            // 1. Handle Deletion Request
            if (actionType == "delete")
            {
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, "uploads", user.ImagePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    user.ImagePath = null;
                }
            }
            // 2. Handle Upload/Change Request (only if a file was actually submitted with the form)
            else if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Delete the old file before saving the new one
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    var oldFile = Path.Combine(uploadsFolder, user.ImagePath);
                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }
                user.ImagePath = uniqueFileName;
            }

            // --- Update Text-based Profile Data ---
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.City = model.City;
            user.Country = model.Country;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                model.ImagePath = user.ImagePath; // Ensure the image path is correct on failure
                return View("Profile", model);
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        // POST: /Person/UploadImage
        // This action handles ONLY the immediate upload/change of a photo.
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UploadImage(IFormFile ProfileImage)
        {
            if (ProfileImage == null || ProfileImage.Length == 0)
            {
                TempData["Error"] = "No file was selected.";
                return RedirectToAction("Profile");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Delete the old file before saving the new one
            if (!string.IsNullOrEmpty(user.ImagePath))
            {
                var oldFile = Path.Combine(uploadsFolder, user.ImagePath);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }
            user.ImagePath = uniqueFileName;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profile image updated!";
            return RedirectToAction("Profile");
        }


        // --- يجب أن تكون لديك نسخة واحدة فقط من هذه الدالة ---
        // POST: /Person/UploadProfileImage
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UploadProfileImage(IFormFile ProfileImage)
        {
            if (ProfileImage == null || ProfileImage.Length == 0)
            {
                TempData["Error"] = "No file selected.";
                return RedirectToAction("Profile");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (!string.IsNullOrEmpty(user.ImagePath))
            {
                var oldFile = Path.Combine(uploadsFolder, user.ImagePath);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }
            user.ImagePath = uniqueFileName;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profile image updated!";
            return RedirectToAction("Profile");
        }

        [Authorize(Roles = "Client")]
        public ActionResult Orders()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            // جلب بيانات الـ Person المرتبط بالمستخدم
            var person = _context.Persons
                .Include(p => p.User)
                .FirstOrDefault(p => p.User != null && p.User.UserName == userName);

            if (person == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // جلب الطلبات التابعة للمستخدم
            var orders = _context.Orders
                .Include(o => o.Persons)
                    .ThenInclude(p => p.User)
                .Include(o => o.Providers)
                    .ThenInclude(p => p.User)
                .Include(o => o.Providers.Services)
                .Where(o => o.PersonId == person.PersonsId)
                .ToList();

            // جلب معرفات الطلبات التي تم تقييمها من قبل المستخدم الحالي
            var ratedOrderIds = _context.Ratings
                .Where(r => r.PersonsId == person.PersonsId)
                .Select(r => r.OrdersId)
                .ToList();

            // إعداد الـ ViewModel
            var model = new VMPersonIndex
            {
                PersonInfo = person,
                MyOrders = orders,
                RatedOrderIds = ratedOrderIds
            };

            return View(model);
        }


        public ActionResult ListOfProvider()
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




            var providers = _context.Providers.Include(p => p.User).ToList();
            return View(providers);
        }



    }
}