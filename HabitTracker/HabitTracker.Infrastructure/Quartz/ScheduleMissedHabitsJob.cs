using HabitTracker.Application.Features.Habits.Commands.CreateMissedHabitLogs;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HabitTracker.Infrastructure.Quartz
{
    public class ScheduleMissedHabitsJob : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScheduleMissedHabitsJob> _logger;

        public ScheduleMissedHabitsJob(IServiceScopeFactory scopeFactory, ILogger<ScheduleMissedHabitsJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("[Quartz] Triggered Hangfire job at {Time}", DateTime.Now);

            BackgroundJob.Enqueue(() => ExecuteCommandViaMediator());
            return Task.CompletedTask;
        }

        [Hangfire.Queue("default")]
        public async Task ExecuteCommandViaMediator()
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new CreateMissedHabitLogsCommand());
        }
    }
}
