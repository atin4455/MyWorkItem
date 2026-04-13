using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWorkItem.Data;
using MyWorkItem.Models;

namespace MyWorkItem.Controllers
{
    public class AdminWorkItemsController(AppDbContext db) : Controller
    {
        private IActionResult? EnsureAdmin()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (role != "Admin")
            {
                return Forbid();
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            var items = await db.WorkItems
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;
            return View(new WorkItem());
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkItem item)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            if (string.IsNullOrWhiteSpace(item.Title))
            {
                ModelState.AddModelError("Title", "標題為必填。");
            }

            if (!ModelState.IsValid)
            {
                return View(item);
            }

            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            item.IsActive = true;
            db.WorkItems.Add(item);
            await db.SaveChangesAsync();

            TempData["Message"] = "新增成功。";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            var item = await db.WorkItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, WorkItem form)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            var item = await db.WorkItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            if (string.IsNullOrWhiteSpace(form.Title))
            {
                ModelState.AddModelError("Title", "標題為必填。");
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            item.Title = form.Title;
            item.Description = form.Description;
            item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            TempData["Message"] = "更新成功。";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            var item = await db.WorkItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                TempData["Error"] = "資料不存在。";
                return RedirectToAction(nameof(Index));
            }

            item.IsActive = false;
            item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            TempData["Message"] = "刪除成功。";
            return RedirectToAction(nameof(Index));
        }
    }
}
