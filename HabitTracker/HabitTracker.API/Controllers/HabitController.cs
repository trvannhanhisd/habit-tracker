using HabitTracker.API.Models;
using HabitTracker.Application.Common.DTO.HabitLog;
using HabitTracker.Application.Features.HabitLogs.Commands.MarkHabitDone;
using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs;
using HabitTracker.Application.Features.Habits.Commands.ArchiveHabit;
using HabitTracker.Application.Features.Habits.Commands.CreateHabit;
using HabitTracker.Application.Features.Habits.Commands.DeleteHabit;
using HabitTracker.Application.Features.Habits.Commands.UpdateHabit;
using HabitTracker.Application.Features.Habits.Queries.GetHabitById;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Application.Features.Habits.Queries.GetHabitsByUser;
using HabitTracker.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitController : ApiControllerBase
    {

        private readonly ILogger<HabitController> _logger;

        public HabitController(ILogger<HabitController> logger)
        {
            _logger = logger;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllHabits()
        {
            _logger.LogInformation("Getting all habits at {time}", DateTime.Now);
            var habits = await Mediator.Send(new GetHabitQuery());
            var response = new ApiResponse<List<HabitViewModel>>(habits);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{habitId}")]
        public async Task<IActionResult> GetHabitById(int habitId)
        {
            _logger.LogInformation("Getting habit at {time}", DateTime.Now);
            var habit = await Mediator.Send(new GetHabitByIdQuery() { HabitId = habitId });
            var response = new ApiResponse<HabitViewModel>(habit);
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpGet("user")]
        public async Task<IActionResult> GetHabitsByUser()
        {
            _logger.LogInformation("Getting all habits of user at {time}", DateTime.Now);
            var userId = User.GetUserId();
            if (userId == 0)
            {
                _logger.LogWarning("Unauthorized attempt to get habits for user {userId} done", userId);
                var errorResponse = new ApiResponse<object>( $"Unauthorized attempt to get habits for user {userId} done", 401);
                return Unauthorized(errorResponse);
            }


            var habits = await Mediator.Send(new GetHabitsByUserQuery { UserId = userId });
            var response = new ApiResponse<List<HabitViewModel>>(habits);
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateHabit(CreateHabitCommand command)
        {
            _logger.LogInformation("Creating habit at {time}", DateTime.Now);
            var createdHabit = await Mediator.Send(command);
            var response = new ApiResponse<HabitViewModel>(createdHabit, 201);
            return CreatedAtAction(nameof(GetHabitById), new { habitId = createdHabit.Id }, response);
        }

        [Authorize(Roles = "User")]
        [HttpPost("{habitId}/done")]
        public async Task<IActionResult> MarkHabitDone(int habitId)
        {
            _logger.LogInformation("Marking habit {HabitId} done at {Time}", habitId, DateTime.UtcNow);
            var command = new MarkHabitDoneCommand
            {
                HabitId = habitId,
                Date = DateTime.UtcNow
            };

            var createdHabitLog = await Mediator.Send(command);

            var response = new ApiResponse<HabitLogViewModel>(createdHabitLog, 201);
            return CreatedAtAction(
                actionName: nameof(HabitLogController.GetHabitLogById),
                controllerName: "HabitLog",
                routeValues: new { habitLogId = createdHabitLog.Id },
                response);
        }

        [Authorize]
        [HttpPut("{habitId}")]
        public async Task<IActionResult> UpdateHabit(int habitId, UpdateHabitCommand command)
        {
            _logger.LogInformation("Updating habit at {time}", DateTime.Now);
            if (habitId != command.Id)
            {
                return BadRequest($"Habit with ID {habitId} not found.");
            }
            await Mediator.Send(command);

            return Ok(new ApiResponse<object>(null, 204));
        }

        [Authorize(Roles = "User")]
        [HttpPatch("{habitId}/archive")]
        public async Task<IActionResult> ArchiveHabit(int habitId)
        {
            _logger.LogInformation("Updating habit at {time}", DateTime.Now);

            await Mediator.Send(new ArchiveHabitCommand() { HabitId = habitId });

            return Ok(new ApiResponse<object>(null, 204));
        }

        [Authorize]
        [HttpDelete("{habitId}")]
        public async Task<IActionResult> DeleteHabit(int habitId)
        {
            _logger.LogInformation("Deleting habit {HabitId} at {Time}", habitId, DateTime.UtcNow);
            await Mediator.Send(new DeleteHabitCommand { HabitId = habitId });
            return Ok(new ApiResponse<object>(null, 204));
        }
    }
}
