using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HabitTracker.Domain.Repository;
using HabitTracker.Infrastructure.Data;
using HabitTracker.Infrastructure.Repository; // THÊM ĐÂY
using Microsoft.EntityFrameworkCore;
using HabitTracker.Domain.Services;
using HabitTracker.Infrastructure.Services;

namespace HabitTracker.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HabitDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<HabitDbContext>());

            services.AddScoped<IHabitRepository, HabitRepository>();
            services.AddScoped<IHabitLogRepository, HabitLogRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }
    }
}