using MyWorkItem.Models;

namespace MyWorkItem.Services
{
    public interface IAdminWorkItemService
    {
        Task<List<WorkItem>> GetAllItemsAsync();
        Task<WorkItem?> GetByIdAsync(int id);
        Task CreateAsync(WorkItem item);
        Task<bool> UpdateAsync(int id, WorkItem form);
        Task<bool> DeleteAsync(int id);
    }
}
