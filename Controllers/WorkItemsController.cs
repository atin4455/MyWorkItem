using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWorkItem.Data;
using MyWorkItem.Models;

namespace MyWorkItem.Controllers
{
    public class WorkItemsController(AppDbContext db) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string sort = "desc")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var query = db.WorkItems.Where(x => x.IsActive);
            var items = sort == "asc"
                ? await query.OrderBy(x => x.CreatedAt).ToListAsync()
                : await query.OrderByDescending(x => x.CreatedAt).ToListAsync();

            var statusMap = await db.UserWorkItemStatuses
                .Where(x => x.UserId == userId.Value)
                .ToDictionaryAsync(x => x.WorkItemId, x => x.IsConfirmed);

            ViewBag.Sort = sort;
            ViewBag.StatusMap = statusMap;
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string sort = "desc")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var item = await db.WorkItems.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            if (item == null)
            {
                return NotFound();
            }

            var userStatus = await db.UserWorkItemStatuses
                .FirstOrDefaultAsync(x => x.UserId == userId.Value && x.WorkItemId == id);
            ViewBag.CurrentStatus = userStatus?.IsConfirmed == true ? "已確認" : "待確認";
            ViewBag.Sort = sort;

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int[] selectedIds, string sort = "desc")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (selectedIds.Length == 0)
            {
                TempData["Error"] = "請至少選擇一個項目。";
                return RedirectToAction(nameof(Index), new { sort });
            }

            foreach (var itemId in selectedIds.Distinct())
            {
                var status = await db.UserWorkItemStatuses
                    .FirstOrDefaultAsync(x => x.UserId == userId.Value && x.WorkItemId == itemId);

                if (status == null)
                {
                    status = new UserWorkItemStatus
                    {
                        UserId = userId.Value,
                        WorkItemId = itemId
                    };
                    db.UserWorkItemStatuses.Add(status);
                }

                status.IsConfirmed = true;
                status.ConfirmedAt = DateTime.UtcNow;
                status.UpdatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
            TempData["Message"] = $"已成功確認 {selectedIds.Distinct().Count()} 項目。";
            return RedirectToAction(nameof(Index), new { sort });
        }

        [HttpPost]
        public async Task<IActionResult> Revoke(int itemId, string sort = "desc")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var status = await db.UserWorkItemStatuses
                .FirstOrDefaultAsync(x => x.UserId == userId.Value && x.WorkItemId == itemId);
            if (status == null)
            {
                TempData["Error"] = "找不到可撤銷的狀態。";
                return RedirectToAction(nameof(Index), new { sort });
            }

            status.IsConfirmed = false;
            status.ConfirmedAt = null;
            status.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            TempData["Message"] = "已將項目標記回待確認。";
            return RedirectToAction(nameof(Index), new { sort });
        }
    }
}
