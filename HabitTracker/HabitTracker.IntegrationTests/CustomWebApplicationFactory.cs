using HabitTracker.API;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Services;
using HabitTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace HabitTracker.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            builder.UseEnvironment("Testing");
            builder.UseContentRoot(Path.GetFullPath("../../../../HabitTracker.API"));
            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContextOptions for HabitDbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<HabitDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Use InMemory DB
                services.AddDbContext<HabitDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });

                // Configure test authentication and set it as default
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                // Replace IUserContext with a fake one used by app code
                var userContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserContext));
                if (userContextDescriptor != null)
                    services.Remove(userContextDescriptor);
                services.AddScoped<IUserContext, FakeUserContext>();

                // Seed DB
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<HabitDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Users.Add(new User
                {
                    Id = 1,
                    UserName = "TestUser",
                    Email = "test@example.com",
                    PasswordHash = "ignored",
                    Role = UserRole.User
                });
                db.SaveChanges();
            });
        }
    }


}
