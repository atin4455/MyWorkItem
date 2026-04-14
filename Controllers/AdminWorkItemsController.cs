using Microsoft.AspNetCore.Mvc;
using MyWorkItem.Models;
using MyWorkItem.Services;

namespace MyWorkItem.Controllers
{
    public class AdminWorkItemsController(IAdminWorkItemService adminWorkItemService) : Controller
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

            var items = await adminWorkItemService.GetAllItemsAsync();
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

            await adminWorkItemService.CreateAsync(item);

            TempData["Message"] = "新增成功。";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            var item = await adminWorkItemService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, WorkItem form)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            if (string.IsNullOrWhiteSpace(form.Title))
            {
                ModelState.AddModelError("Title", "標題為必填。");
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var updated = await adminWorkItemService.UpdateAsync(id, form);
            if (!updated) return NotFound();

            TempData["Message"] = "更新成功。";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            var deleted = await adminWorkItemService.DeleteAsync(id);
            if (!deleted)
            {
                TempData["Error"] = "資料不存在。";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "刪除成功。";
            return RedirectToAction(nameof(Index));
        }
    }
}
