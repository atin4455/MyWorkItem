using Microsoft.EntityFrameworkCore;
using MyWorkItem.Data;
using MyWorkItem.Models;

namespace MyWorkItem.Services
{
    public class AdminWorkItemService(AppDbContext dbContext) : IAdminWorkItemService
    {
        public Task<List<WorkItem>> GetAllItemsAsync()
        {
            return dbContext.WorkItems.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public Task<WorkItem?> GetByIdAsync(int id)
        {
            return dbContext.WorkItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(WorkItem item)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            item.IsActive = true;
            await dbContext.WorkItems.AddAsync(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, WorkItem form)
        {
            var item = await dbContext.WorkItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return false;
            }

            item.Title = form.Title;
            item.Description = form.Description;
            item.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await dbContext.WorkItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return false;
            }

            dbContext.WorkItems.Remove(item);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
