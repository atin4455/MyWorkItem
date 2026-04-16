using Microsoft.AspNetCore.Mvc;
using MyWorkItem.Dtos;
using MyWorkItem.Services;
using MyWorkItem.ViewModels.AdminWorkItems;

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

            var readDtos = await adminWorkItemService.GetAllItemsAsync();
            var vm = readDtos
                .Select(dto => new AdminWorkItemListItemViewModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    UpdatedAt = dto.UpdatedAt,
                    IsActive = dto.IsActive
                })
                .ToList();
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;
            return View(new AdminWorkItemFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminWorkItemFormViewModel model)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await adminWorkItemService.CreateAsync(new AdminWorkItemWriteDto
            {
                Title = model.Title,
                Description = model.Description
            });

            TempData["Message"] = "新增成功。";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            var readDto = await adminWorkItemService.GetByIdAsync(id);
            if (readDto == null) return NotFound();

            return View(new AdminWorkItemFormViewModel
            {
                Id = readDto.Id,
                Title = readDto.Title,
                Description = readDto.Description
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AdminWorkItemFormViewModel model)
        {
            var guard = EnsureAdmin();
            if (guard != null) return guard;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updated = await adminWorkItemService.UpdateAsync(id, new AdminWorkItemWriteDto
            {
                Title = model.Title,
                Description = model.Description
            });
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
