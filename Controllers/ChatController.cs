using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;
using System.Security.Claims;

namespace Ostawy.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? requestId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);
            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();

            var conversations = _context.ChatMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    OtherUserId = g.Key,
                    OtherUserName = _context.Users.Where(u => u.Id == g.Key).Select(u => u.FullName).FirstOrDefault(),
                    LastMessage = g.OrderByDescending(m => m.CreatedAt).First().Message,
                    LastMessageDate = g.OrderByDescending(m => m.CreatedAt).First().CreatedAt,
                    UnreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead),
                    RequestId = g.OrderByDescending(m => m.CreatedAt).First().JobRequestId
                })
                .OrderByDescending(c => c.LastMessageDate)
                .ToList();

            ViewBag.Conversations = conversations;
            ViewBag.CurrentUserId = userId;
            ViewBag.RequestId = requestId;
            return View();
        }

        [HttpGet]
        public IActionResult GetMessages(int otherUserId, int? requestId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            int userId = int.Parse(userIdStr);

            var messages = _context.ChatMessages
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == userId))
                .Where(m => !requestId.HasValue || m.JobRequestId == requestId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.Message,
                    m.CreatedAt,
                    m.IsRead
                })
                .ToList();

            var unread = _context.ChatMessages
                .Where(m => m.SenderId == otherUserId && m.ReceiverId == userId && !m.IsRead);
            foreach (var msg in unread) msg.IsRead = true;
            _context.SaveChanges();

            return Json(new { messages, currentUserId = userId });
        }

        [HttpPost]
        public IActionResult SendMessage(int receiverId, string message, int? requestId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Json(new { success = false });

            int userId = int.Parse(userIdStr);

            var chatMsg = new ChatMessage
            {
                SenderId = userId,
                ReceiverId = receiverId,
                JobRequestId = requestId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.ChatMessages.Add(chatMsg);
            _context.SaveChanges();

            return Json(new { success = true, message = "تم إرسال الرسالة" });
        }
    }
}
