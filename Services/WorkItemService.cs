using Microsoft.EntityFrameworkCore;
using MyWorkItem.Data;
using MyWorkItem.Dtos;
using MyWorkItem.Models;

namespace MyWorkItem.Services
{
    public class WorkItemService(AppDbContext dbContext) : IWorkItemService
    {
        public async Task<List<WorkItemReadDto>> GetActiveItemsAsync(string sort)
        {
            var query = dbContext.WorkItems.Where(entity => entity.IsActive);
            var ordered = sort == "asc"
                ? query.OrderBy(entity => entity.CreatedAt)
                : query.OrderByDescending(entity => entity.CreatedAt);

            return await ordered
                .Select(entity => new WorkItemReadDto
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt
                })
                .ToListAsync();
        }

        public Task<Dictionary<int, bool>> GetUserStatusMapAsync(int userId)
        {
            return dbContext.UserWorkItemStatuses
                .Where(entity => entity.UserId == userId)
                .ToDictionaryAsync(entity => entity.WorkItemId, entity => entity.IsConfirmed);
        }

        public Task<WorkItemReadDto?> GetActiveItemByIdAsync(int id)
        {
            return dbContext.WorkItems
                .Where(entity => entity.Id == id && entity.IsActive)
                .Select(entity => new WorkItemReadDto
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetCurrentStatusTextAsync(int userId, int workItemId)
        {
            var statusEntity = await dbContext.UserWorkItemStatuses
                .FirstOrDefaultAsync(entity => entity.UserId == userId && entity.WorkItemId == workItemId);
            return statusEntity?.IsConfirmed == true ? "已確認" : "待確認";
        }

        public async Task<int> ConfirmItemsAsync(int userId, int[] selectedIds)
        {
            var distinctIds = selectedIds.Distinct().ToArray();
            foreach (var workItemId in distinctIds)
            {
                var statusEntity = await dbContext.UserWorkItemStatuses
                    .FirstOrDefaultAsync(entity => entity.UserId == userId && entity.WorkItemId == workItemId);
                if (statusEntity == null)
                {
                    statusEntity = new UserWorkItemStatus
                    {
                        UserId = userId,
                        WorkItemId = workItemId
                    };
                    await dbContext.UserWorkItemStatuses.AddAsync(statusEntity);
                }

                statusEntity.IsConfirmed = true;
                statusEntity.ConfirmedAt = DateTime.UtcNow;
                statusEntity.UpdatedAt = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();
            return distinctIds.Length;
        }

        public async Task<bool> RevokeItemAsync(int userId, int itemId)
        {
            var statusEntity = await dbContext.UserWorkItemStatuses
                .FirstOrDefaultAsync(entity => entity.UserId == userId && entity.WorkItemId == itemId);
            if (statusEntity == null)
            {
                return false;
            }

            statusEntity.IsConfirmed = false;
            statusEntity.ConfirmedAt = null;
            statusEntity.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
