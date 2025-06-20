using HomeServices.Models;
using HomeServices.ViewModels;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeServices.Enums;

namespace HomeServices.Controllers
{
    public class OrderController : Controller
    {

        IRepositorie<Orders, int> _rep;
        private readonly AppDBContext _context;
        private readonly UserManager<Users> _userManager;



        public OrderController(
            IRepositorie<Orders, int> rep,
            AppDBContext context,
            UserManager<Users> userManager)

        {
            _rep = rep;
            _context = context;
            _userManager = userManager;
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
        [HttpGet]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> Reservation()
        {
            // احصل على المستخدم الحالي
            var user = await _userManager.GetUserAsync(User);

            // احصل على الـ Provider المرتبط بالمستخدم الحالي
            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (provider == null)
            {
                return NotFound();
            }

            // احصل على الحجوزات التي تخص هذا الـ Provider فقط
            var orders = await _context.Orders
                .Include(o => o.Persons)
                    .ThenInclude(p => p.User)
                .Include(o => o.Providers)
                    .ThenInclude(p => p.Services)
                .Where(o => o.DeletedAt == null && o.ProviderId == provider.ProvidersId)
                .ToListAsync();

            return View(orders);
        }


        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> BookingByDate(string date)
        {

            if (!DateTime.TryParse(date, out var selectedDate))
            {

                return BadRequest("Invalid date format.");
            }

            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");




            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }


            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (provider == null)
            {

                return NotFound("Provider profile not found for the current user.");
            }




            var bookings = await _context.Orders
                .Include(o => o.Persons).ThenInclude(p => p.User)
                .Include(o => o.Providers).ThenInclude(p => p.Services)
                .Where(o => o.ProviderId == provider.ProvidersId &&
                            o.OrdersDate.Date == selectedDate.Date)
                .ToListAsync();

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(request.Id);
            if (order == null)
                return NotFound();

            if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var newStatus))
                return BadRequest("Invalid status value.");

            order.Status = newStatus.ToString();
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok();
        }



        [Authorize(Roles = "Provider")]
        public IActionResult BookingDetails(int id)
        {


            var order = _context.Orders
     .Include(o => o.Providers)
         .ThenInclude(p => p.User)
     .Include(o => o.Providers)
         .ThenInclude(p => p.Services)
     .Include(o => o.Persons)
         .ThenInclude(p => p.User)
     .FirstOrDefault(o => o.OrdersId == id);


            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new VMCompletedOrder
            {
                OrderId = order.OrdersId,
                OrdersDate = order.OrdersDate,
                OrdersTime = order.OrdersTime,
                ProviderName = $"{order.Providers.User?.FirstName} {order.Providers.User?.LastName}",
                PersonName = $"{order.Persons.User?.FirstName} {order.Persons.User?.LastName}",
                ServiceName = order.Providers.Services.ServiceName,
                Address = order.Address,
                MapUrl = order.MapUrl,
                Description = order.Description,
                Status = order.Status
            };

            return View(viewModel);


        }

        

        [Authorize(Roles = "Provider")] 
        public async Task<IActionResult> CompletedOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            
            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (provider == null)
            {
                
                return NotFound("Provider profile not found for the current user.");
            }
            var completedOrders = await _context.Orders
                .Include(o => o.Providers)
                    .ThenInclude(p => p.User)
                .Include(o => o.Providers)
                    .ThenInclude(p => p.Services)
                .Include(o => o.Persons)
                    .ThenInclude(p => p.User)
               
                .Where(o =>
                    o.ProviderId == provider.ProvidersId && 
                    o.Status.ToLower() == "completed" &&
                    o.DeletedAt == null)
                .AsSplitQuery() 
                .ToListAsync();

            
            var viewModel = completedOrders.Select(order => new VMCompletedOrder
            {
                OrderId = order.OrdersId,
                Address = order.Address,
                MapUrl = order.MapUrl,
                OrdersDate = order.OrdersDate,
                OrdersTime = order.OrdersTime,
                Status = order.Status,
                Description = order.Description,
                ProviderName = (order.Providers?.User?.FirstName ?? "") + " " + (order.Providers?.User?.LastName ?? ""),
                PersonName = (order.Persons?.User?.FirstName ?? "") + " " + (order.Persons?.User?.LastName ?? ""),
                ServiceName = order.Providers?.Services?.ServiceName ?? "N/A"
            }).ToList();

            
            return View(viewModel);
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
