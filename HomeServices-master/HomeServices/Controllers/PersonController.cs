using HomeServices.Models;
using HomeServices.Models.Repositorie;
using HomeServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
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

        public PersonController(
            IRepositorie<Persons, int> rep,
            IRepositorie<Providers, int> providerRep,
            IRepositorie<Orders, int> orderRep,
            IHttpContextAccessor httpContextAccessor,
            UserManager<Users> userManager,
            AppDBContext context)
        {
            _rep = rep;
            _providerRep = providerRep;
            _orderRep = orderRep;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
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

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> EditProfile(Persons model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return NotFound();

            // تحديث البيانات
            user.FirstName = model.User.FirstName;
            user.LastName = model.User.LastName;
            user.PhoneNumber = model.User.PhoneNumber;
            user.Email = model.User.Email;
            user.City = model.User.City;
            user.Country = model.User.Country;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Profile");
        }


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
            var currentPerson = _rep.View().FirstOrDefault(p => p.User.UserName == userName);

            var providers = _providerRep.View().ToList();

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
        public ActionResult Profile()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            var currentPerson = _rep.View().FirstOrDefault(p => p.User.UserName == userName);

            return View(currentPerson);

        }

        [Authorize(Roles = "Client")]
        public ActionResult Orders()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            var orders = _context.Orders
                .Include(o => o.Persons)
                    .ThenInclude(p => p.User)
                .Include(o => o.Providers)
                    .ThenInclude(p => p.User)
                .Include(o => o.Providers.Services)
                .Where(o => o.Persons != null && o.Persons.User != null && o.Persons.User.UserName == userName)
                .ToList();



            var model = new VMPersonIndex
            {
                MyOrders = orders
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