using Microsoft.AspNetCore.Mvc;
using Ostawy.Data;
using Ostawy.Models;
using System.Linq;

namespace Ostawy.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. أكشن يشتغل لما العميل يدوس "احجز الآن" من الخريطة
        [HttpPost]
        public IActionResult CreateOrder(int workerId, int clientId)
        {
            var newOrder = new Order
            {
                ClientId = clientId,
                WorkerId = workerId,
                Status = "Pending" // هيروح للفني كـ معلق
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            return Json(new { success = true, message = "تم إرسال الطلب للفني بنجاح! 🚀" });
        }

        // 2. أكشن يشتغل لما الفني يدوس "قبول" في لوحة تحكمه
        [HttpPost]
        public IActionResult AcceptOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = "Accepted";
                _context.SaveChanges();
                return Json(new { success = true, message = "تم قبول الطلب، توجه للعميل! 🛠️" });
            }
            return NotFound();
        }

        // 3. أكشن يشتغل لما الفني يدوس "رفض"
        [HttpPost]
        public IActionResult RejectOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = "Rejected";
                _context.SaveChanges();
                return Json(new { success = true, message = "تم رفض الطلب." });
            }
            return NotFound();
        }
    }
}