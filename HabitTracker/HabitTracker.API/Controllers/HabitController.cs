using HabitTracker.Application.Features.Habits.Commands.ArchiveHabit;
using HabitTracker.Application.Features.Habits.Commands.CreateHabit;
using HabitTracker.Application.Features.Habits.Commands.DeleteHabit;
using HabitTracker.Application.Features.Habits.Commands.UpdateHabit;
using HabitTracker.Application.Features.Habits.Queries.GetHabitById;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Application.Features.Habits.Queries.GetHabitsByUser;
using HabitTracker.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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


        [HttpGet]
        public async Task<IActionResult> GetAllHabits()
        {
            _logger.LogInformation("Getting all habits at {time}", DateTime.Now);
            var habits = await Mediator.Send(new GetHabitQuery());
            return Ok(habits);
        }

        [HttpGet("{habitId}")]
        public async Task<IActionResult> GetHabitById(int habitId)
        {
            _logger.LogInformation("Getting habit at {time}", DateTime.Now);
            var habit = await Mediator.Send(new GetHabitByIdQuery() { HabitId = habitId });
            if(habit == null)
            {
                return NotFound($"Habit with ID {habitId} not found.");
            }
            return Ok(habit);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetHabitsByUser(int userId)
        {
            _logger.LogInformation("Getting all habits of user {userId} at {time}", userId, DateTime.Now);
            var habits = await Mediator.Send(new GetHabitsByUserQuery { UserId = userId }); 
            return Ok(habits);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHabit(CreateHabitCommand command)
        {
            _logger.LogInformation("Creating habit at {time}", DateTime.Now);
            var createdHabit = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetHabitById), new { habitId = createdHabit.Id }, createdHabit);
        }

        [HttpPut("{habitId}")]
        public async Task<IActionResult> UpdateHabit(int habitId, UpdateHabitCommand command)
        {
            _logger.LogInformation("Updating habit at {time}", DateTime.Now);
            if (habitId != command.Id)
            {
                return BadRequest($"Habit with ID {habitId} not found.");
            }
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{habitId}/archive")]
        public async Task<IActionResult> ArchiveHabit(int habitId)
        {
            _logger.LogInformation("Updating habit at {time}", DateTime.Now);

            await Mediator.Send(new ArchiveHabitCommand() { HabitId = habitId });

            return NoContent();
        }

        [HttpDelete("{habitId}")]
        public async Task<IActionResult> DeleteHabit(int habitId, DeleteHabitCommand command)
        {
            _logger.LogInformation("Deleting habit at {time}", DateTime.Now);
            if (habitId != command.Id)
            {
                return BadRequest($"Habit with ID {habitId} not found.");
            }
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
