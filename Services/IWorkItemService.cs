using MyWorkItem.Models;

namespace MyWorkItem.Services
{
    public interface IWorkItemService
    {
        Task<List<WorkItem>> GetActiveItemsAsync(string sort);
        Task<Dictionary<int, bool>> GetUserStatusMapAsync(int userId);
        Task<WorkItem?> GetActiveItemByIdAsync(int id);
        Task<string> GetCurrentStatusTextAsync(int userId, int workItemId);
        Task<int> ConfirmItemsAsync(int userId, int[] selectedIds);
        Task<bool> RevokeItemAsync(int userId, int itemId);
    }
}
