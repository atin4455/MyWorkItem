using Microsoft.EntityFrameworkCore;
using MyWorkItem.Data;
using MyWorkItem.Models;

namespace MyWorkItem.Services
{
    public class WorkItemService(AppDbContext dbContext) : IWorkItemService
    {
        public async Task<List<WorkItem>> GetActiveItemsAsync(string sort)
        {
            var query = dbContext.WorkItems.Where(x => x.IsActive);
            return sort == "asc"
                ? await query.OrderBy(x => x.CreatedAt).ToListAsync()
                : await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public Task<Dictionary<int, bool>> GetUserStatusMapAsync(int userId)
        {
            return dbContext.UserWorkItemStatuses
                .Where(x => x.UserId == userId)
                .ToDictionaryAsync(x => x.WorkItemId, x => x.IsConfirmed);
        }

        public Task<WorkItem?> GetActiveItemByIdAsync(int id)
        {
            return dbContext.WorkItems.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<string> GetCurrentStatusTextAsync(int userId, int workItemId)
        {
            var status = await dbContext.UserWorkItemStatuses
                .FirstOrDefaultAsync(x => x.UserId == userId && x.WorkItemId == workItemId);
            return status?.IsConfirmed == true ? "已確認" : "待確認";
        }

        public async Task<int> ConfirmItemsAsync(int userId, int[] selectedIds)
        {
            var distinctIds = selectedIds.Distinct().ToArray();
            foreach (var itemId in distinctIds)
            {
                var status = await dbContext.UserWorkItemStatuses
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.WorkItemId == itemId);
                if (status == null)
                {
                    status = new UserWorkItemStatus
                    {
                        UserId = userId,
                        WorkItemId = itemId
                    };
                    await dbContext.UserWorkItemStatuses.AddAsync(status);
                }

                status.IsConfirmed = true;
                status.ConfirmedAt = DateTime.UtcNow;
                status.UpdatedAt = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();
            return distinctIds.Length;
        }

        public async Task<bool> RevokeItemAsync(int userId, int itemId)
        {
            var status = await dbContext.UserWorkItemStatuses
                .FirstOrDefaultAsync(x => x.UserId == userId && x.WorkItemId == itemId);
            if (status == null)
            {
                return false;
            }

            status.IsConfirmed = false;
            status.ConfirmedAt = null;
            status.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
