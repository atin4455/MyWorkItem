using MyWorkItem.Dtos;

namespace MyWorkItem.Services
{
    public interface IAdminWorkItemService
    {
        Task<List<AdminWorkItemReadDto>> GetAllItemsAsync();
        Task<AdminWorkItemReadDto?> GetByIdAsync(int id);
        Task CreateAsync(AdminWorkItemWriteDto writeDto);
        Task<bool> UpdateAsync(int id, AdminWorkItemWriteDto writeDto);
        Task<bool> DeleteAsync(int id);
    }
}
