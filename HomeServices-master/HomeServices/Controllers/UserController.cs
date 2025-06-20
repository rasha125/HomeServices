using HomeServices.Data;
using HomeServices.Models;
using HomeServices.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeServices.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;

        public UsersController(AppDBContext context, UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                ServicesList = _context.Services
                    .Select(s => new SelectListItem
                    {
                        Value = s.ServicesId.ToString(),
                        Text = s.ServiceName
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.ServicesList = _context.Services
         .Select(s => new SelectListItem
         {
             Value = s.ServicesId.ToString(),
             Text = s.ServiceName
         }).ToList();

            // إذا المستخدم Client، احذف الحقول الخاصة بـ Provider من ModelState
            if (model.Role != UserRole.Provider)
            {
                ModelState.Remove(nameof(model.Age));
                ModelState.Remove(nameof(model.Description));
                ModelState.Remove(nameof(model.ServicesId));
            }

            if (!ModelState.IsValid)
                return View(model);

            if (model.Role == UserRole.Provider)
            {
                if (!model.Age.HasValue)
                    ModelState.AddModelError("Age", "Age is required for providers.");
                if (!model.ServicesId.HasValue)
                    ModelState.AddModelError("ServicesId", "Service is required for providers.");

                if (!ModelState.IsValid)
                    return View(model);
            }

            var user = new Users
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                City = model.City,
                Country = model.Country,
                RoleId = (int)model.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.Role.ToString());

            if (model.Role == UserRole.Client)
            {
                var person = new Persons
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Persons.Add(person);
            }
            else if (model.Role == UserRole.Provider)
            {
                var provider = new Providers
                {
                    UserId = user.Id,
                    Age = model.Age.Value,
                    ServicesId = model.ServicesId.Value,
                    ProviderStatus = "Available",
                    Description = model.Description, // ✅ استخدم وصف المستخدم وليس نص ثابت
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Providers.Add(provider);
            }

            await _context.SaveChangesAsync();
            await _signInManager.SignInAsync(user, isPersistent: false);

            if (model.Role == UserRole.Client)
            {
                return RedirectToAction("dashboard", "Person");
            }
            else if (model.Role == UserRole.Provider)
            {
                return RedirectToAction("dashboard", "Provider");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Please enter email and password.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                var role = ((UserRole)user.RoleId).ToString();

                if (role == "Admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "Provider")
                    return RedirectToAction("dashboard", "Provider");
                else
                    return RedirectToAction("dashboard", "Person");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Users");
        }
    }
}