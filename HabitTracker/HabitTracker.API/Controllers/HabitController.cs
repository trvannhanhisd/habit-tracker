using HabitTracker.Application.Habits.Commands.CreateHabit;
using HabitTracker.Application.Habits.Commands.DeleteHabit;
using HabitTracker.Application.Habits.Commands.UpdateHabit;
using HabitTracker.Application.Habits.Queries.GetHabitById;
using HabitTracker.Application.Habits.Queries.GetHabits;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHabitById(int habitId)
        {
            var habit = await Mediator.Send(new GetHabitByIdQuery() { HabitId = habitId });
            if(habit == null)
            {
                return NotFound($"Habit with ID {habitId} not found.");
            }
            return Ok(habit);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHabit(CreateHabitCommand command)
        {
            var createdHabit = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetHabitById), new { id = createdHabit.Id }, createdHabit);
        }

        [HttpPut("{habitId}")]
        public async Task<IActionResult> UpdateHabit(int habitId, UpdateHabitCommand command)
        {
            if(habitId != command.Id)
            {
                return BadRequest($"Habit with ID {habitId} not found.");
            }
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{habitId}")]
        public async Task<IActionResult> DeleteHabit(int habitId, DeleteHabitCommand command)
        {
            if (habitId != command.Id)
            {
                return BadRequest($"Habit with ID {habitId} not found.");
            }
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
