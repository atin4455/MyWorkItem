using Microsoft.AspNetCore.Mvc;
using MyWorkItem.Services;
using MyWorkItem.ViewModels.WorkItems;

namespace MyWorkItem.Controllers
{
    public class WorkItemsController(IWorkItemService workItemService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string sort = "desc")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var readDtos = await workItemService.GetActiveItemsAsync(sort);
            var statusMap = await workItemService.GetUserStatusMapAsync(userId.Value);

            var vm = new WorkItemIndexViewModel
            {
                Sort = sort,
                Items = readDtos
                    .Select(dto => new WorkItemListItemViewModel
                    {
                        Id = dto.Id,
                        Title = dto.Title,
                        IsConfirmed = statusMap.TryGetValue(dto.Id, out var confirmed) && confirmed
                    })
                    .ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string sort = "desc")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var readDto = await workItemService.GetActiveItemByIdAsync(id);
            if (readDto == null)
            {
                return NotFound();
            }

            var currentStatus = await workItemService.GetCurrentStatusTextAsync(userId.Value, id);

            return View(new WorkItemDetailViewModel
            {
                Id = readDto.Id,
                Title = readDto.Title,
                Description = readDto.Description,
                CreatedAt = readDto.CreatedAt,
                UpdatedAt = readDto.UpdatedAt,
                CurrentStatus = currentStatus,
                Sort = sort
            });
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

            var count = await workItemService.ConfirmItemsAsync(userId.Value, selectedIds);
            TempData["Message"] = $"已成功確認 {count} 項目。";
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

            var revoked = await workItemService.RevokeItemAsync(userId.Value, itemId);
            if (!revoked)
            {
                TempData["Error"] = "找不到可撤銷的狀態。";
                return RedirectToAction(nameof(Index), new { sort });
            }

            TempData["Message"] = "已將項目標記回待確認。";
            return RedirectToAction(nameof(Index), new { sort });
        }
    }
}
