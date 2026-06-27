using Microsoft.AspNetCore.Mvc;
using Ostawy.Data;
using Ostawy.Models;
using System.Linq;

namespace Ostawy.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // عرض كل الأقسام
        public IActionResult Index()
        {
            var categories = _db.Categories.ToList();
            return View(categories);
        }

        // فتح صفحة الإضافة
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // فتح صفحة التعديل
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var category = _db.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // الحذف
        public IActionResult Delete(int? id)
        {
            var category = _db.Categories.Find(id);
            if (category == null) return NotFound();

            _db.Categories.Remove(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}