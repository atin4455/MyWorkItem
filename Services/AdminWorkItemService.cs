using Microsoft.EntityFrameworkCore;
using MyWorkItem.Data;
using MyWorkItem.Dtos;
using MyWorkItem.Models;

namespace MyWorkItem.Services
{
    public class AdminWorkItemService(AppDbContext dbContext) : IAdminWorkItemService
    {
        public Task<List<AdminWorkItemReadDto>> GetAllItemsAsync()
        {
            return dbContext.WorkItems
                .OrderByDescending(entity => entity.CreatedAt)
                .Select(entity => new AdminWorkItemReadDto
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    IsActive = entity.IsActive
                })
                .ToListAsync();
        }

        public Task<AdminWorkItemReadDto?> GetByIdAsync(int id)
        {
            return dbContext.WorkItems
                .Where(entity => entity.Id == id)
                .Select(entity => new AdminWorkItemReadDto
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    IsActive = entity.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(AdminWorkItemWriteDto writeDto)
        {
            var entity = new WorkItem
            {
                Title = writeDto.Title,
                Description = writeDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await dbContext.WorkItems.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, AdminWorkItemWriteDto writeDto)
        {
            var entity = await dbContext.WorkItems.FirstOrDefaultAsync(entity => entity.Id == id);
            if (entity == null)
            {
                return false;
            }

            entity.Title = writeDto.Title;
            entity.Description = writeDto.Description;
            entity.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await dbContext.WorkItems.FirstOrDefaultAsync(entity => entity.Id == id);
            if (entity == null)
            {
                return false;
            }

            dbContext.WorkItems.Remove(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
