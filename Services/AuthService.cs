using Microsoft.EntityFrameworkCore;
using MyWorkItem.Data;
using MyWorkItem.Models;

namespace MyWorkItem.Services
{
    public class AuthService(AppDbContext dbContext) : IAuthService
    {
        public Task<User?> ValidateUserAsync(string account, string password)
        {
            return dbContext.Users.FirstOrDefaultAsync(x => x.Account == account && x.Password == password);
        }
    }
}
