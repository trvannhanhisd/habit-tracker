using HabitTracker.Application.Features.Habits.Commands.CreateMissedHabitLogs;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using static HabitTracker.Domain.Entity.Habit;

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

            // enqueue background job (chỉ gọi 1 job duy nhất, logic nằm bên trong)
            BackgroundJob.Enqueue(() => ExecuteCommandViaMediator());

            return Task.CompletedTask;
        }

        [Hangfire.Queue("default")]
        public async Task ExecuteCommandViaMediator()
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var now = DateTime.UtcNow;

            //Luôn xử lý Daily habit mỗi ngày
            await mediator.Send(new CreateMissedHabitLogsCommand(HabitFrequency.Daily));

            // Chủ nhật → xử lý Weekly habit
            if (now.DayOfWeek == DayOfWeek.Sunday)
            {
                await mediator.Send(new CreateMissedHabitLogsCommand(HabitFrequency.Weekly));
            }

            // Ngày cuối tháng → xử lý Monthly habit
            if (now.Day == DateTime.DaysInMonth(now.Year, now.Month))
            {
                await mediator.Send(new CreateMissedHabitLogsCommand(HabitFrequency.Monthly));
            }

            _logger.LogInformation("[Hangfire] Finished executing missed habit jobs at {Time}", DateTime.Now);
        }
    }
}
