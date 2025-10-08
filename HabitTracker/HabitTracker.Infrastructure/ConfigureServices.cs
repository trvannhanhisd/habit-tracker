using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HabitTracker.Domain.Repository;
using HabitTracker.Infrastructure.Data;
using HabitTracker.Infrastructure.Repository; // THÊM ĐÂY
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HabitDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IHabitRepository, HabitRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();

            return services;
        }
    }
}