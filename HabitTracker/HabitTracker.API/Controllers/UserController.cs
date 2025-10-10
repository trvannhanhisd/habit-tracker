using HabitTracker.API.Models;
using HabitTracker.Application.Features.Auth.Commands.Login;
using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs;
using HabitTracker.Application.Features.Habits.Commands.UpdateHabit;
using HabitTracker.Application.Features.Habits.Queries.GetHabitById;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Application.Features.Users.Commands.UpdateUser;
using HabitTracker.Application.Features.Users.Queries.GetUserById;
using HabitTracker.Application.Features.Users.Queries.GetUsers;
using HabitTracker.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Getting all users at {time}", DateTime.Now);
            var users = await Mediator.Send(new GetUsersQuery());
            var response = new ApiResponse<List<UserViewModel>>(users);
            return Ok(response);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            _logger.LogInformation("Getting user at {time}", DateTime.Now);
            var user = await Mediator.Send(new GetUserByIdQuery() { UserId = userId });
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }
            var response = new ApiResponse<UserViewModel>(user);
            return Ok(response);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, UpdateUserCommand command)
        {
            _logger.LogInformation("Updating habit at {time}", DateTime.Now);
            if (userId != command.Id)
            {
                return BadRequest($"Habit with ID {userId} not found.");
            }
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
