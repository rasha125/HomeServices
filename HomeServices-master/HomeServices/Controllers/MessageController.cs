using HomeServices.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class MessageController : Controller
{
    private readonly AppDBContext _context;
    private readonly UserManager<Users> _userManager;

    public MessageController(AppDBContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public IActionResult Chat(string receiverId)
    {
        var senderId = _userManager.GetUserId(User);

        var messages = _context.Messages
            .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId)
                     || (m.SenderId == receiverId && m.ReceiverId == senderId))
            .Include(m => m.Sender) // <==== لازم
            .OrderBy(m => m.SentAt)
            .ToList();

        var receiver = _context.Users.FirstOrDefault(u => u.Id == receiverId);
        ViewBag.ReceiverName = receiver != null ? $"{receiver.FirstName} {receiver.LastName}" : "Unknown";


        ViewBag.ReceiverId = receiverId;
        return View(messages);
    }

    [HttpPost]
    public IActionResult Send(string receiverId, string content)
    {
        var senderId = _userManager.GetUserId(User);

        var message = new Messages
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content
        };

        _context.Messages.Add(message);
        _context.SaveChanges();

        return RedirectToAction("Chat", new { receiverId });
    }

    //[HttpPost]
    //public IActionResult Send(string receiverId, string content)
    //{
    //    var senderId = _userManager.GetUserId(User);

    //    // لو ال User هو Provider، نتحقق إنه يستقبل فقط رسائل لـ Persons عندهم طلبات عنده
    //    bool isProvider = User.IsInRole("Provider");

    //    if (isProvider)
    //    {
    //        // نتحقق هل ال receiverId (Person) عنده طلب مع هذا ال Provider (الذي هو sender)
    //        bool hasOrder = _context.Orders.Any(o =>
    //            o.ProviderId == _context.Providers
    //                            .Where(p => p.UserId == senderId)
    //                            .Select(p => p.ProvidersId)
    //                            .FirstOrDefault()
    //            && o.Persons.UserId == receiverId);

    //        if (!hasOrder)
    //        {
    //            // ممنوع يرسل رسالة لهذا الشخص
    //            return Forbid();
    //        }
    //    }

    //    // يسمح بالارسال
    //    var message = new Messages
    //    {
    //        SenderId = senderId,
    //        ReceiverId = receiverId,
    //        Content = content
    //    };

    //    _context.Messages.Add(message);
    //    _context.SaveChanges();

    //    return RedirectToAction("Chat", new { receiverId });
    //}


    public IActionResult Inbox()
    {
        var currentUserId = _userManager.GetUserId(User);

        // نجيب كل الرسائل بين المستخدم الحالي والآخرين
        var allMessages = _context.Messages
            .Where(m => m.ReceiverId == currentUserId || m.SenderId == currentUserId)
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .OrderByDescending(m => m.SentAt)
            .ToList();

        // نختار فقط المحادثات اللي الطرف الآخر أرسللي رسالة وأنا ما رديت عليه بعدها
        var inboxConversations = allMessages
            .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
            .Select(g =>
            {
                var lastMessage = g.OrderByDescending(m => m.SentAt).FirstOrDefault();
                return lastMessage;
            })
            .Where(m => m != null && m.SenderId != currentUserId) // إذا آخر رسالة منه هو، ولسا ما رديت
            .ToList();

        return View(inboxConversations);
    }



}