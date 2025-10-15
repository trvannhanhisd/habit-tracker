using FluentValidation;
using HabitTracker.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HabitTracker.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(ctg =>
            {
                ctg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                ctg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            });

            return services;
            
        }
    }
}