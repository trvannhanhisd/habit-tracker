using HabitTracker.Application.Features.Auth.Commands.Login;
using HabitTracker.Application.Features.Auth.Commands.RefreshToken;
using HabitTracker.Application.Features.Auth.Commands.Register;
using HabitTracker.Application.Features.Habits.Queries.GetHabitById;
using HabitTracker.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserViewModel>> Register(RegisterCommand command)
        {
            var registeredUser = await Mediator.Send(command);
            if (registeredUser == null)
            {
                return BadRequest("User registration failed.");
            }
            return Ok(registeredUser);

        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseViewModel>> Login(LoginCommand command)
        {
            var result = await Mediator.Send(command);
            if (result is null)
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseViewModel>> RefreshToken(RefreshTokenCommand command)
        {
            var result = await Mediator.Send(command);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }


    }
}
