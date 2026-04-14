using Microsoft.Extensions.DependencyInjection;

namespace MyWorkItem.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IWorkItemService, WorkItemService>();
            services.AddScoped<IAdminWorkItemService, AdminWorkItemService>();
            return services;
        }
    }
}
