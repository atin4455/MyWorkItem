using MyWorkItem.Dtos;

namespace MyWorkItem.Services
{
    public interface IWorkItemService
    {
        Task<List<WorkItemReadDto>> GetActiveItemsAsync(string sort);
        Task<Dictionary<int, bool>> GetUserStatusMapAsync(int userId);
        Task<WorkItemReadDto?> GetActiveItemByIdAsync(int id);
        Task<string> GetCurrentStatusTextAsync(int userId, int workItemId);
        Task<int> ConfirmItemsAsync(int userId, int[] selectedIds);
        Task<bool> RevokeItemAsync(int userId, int itemId);
    }
}
