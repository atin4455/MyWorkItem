using MyWorkItem.Models;

namespace MyWorkItem.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string account, string password);
    }
}
