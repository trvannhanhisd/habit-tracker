using HabitTracker.API.Models;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Application.Features.Auth.Commands.Login;
using HabitTracker.Application.Features.Auth.Commands.RefreshToken;
using HabitTracker.Application.Features.Auth.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserViewModel>> Register(RegisterCommand command)
        {
            _logger.LogInformation("Registering user {UserName}", command.UserName);
            var registeredUser = await Mediator.Send(command);

            var response = new ApiResponse<UserViewModel>(registeredUser, 201);
            return CreatedAtAction(
                actionName: nameof(UserController.GetUserById),
                controllerName: "User",
                routeValues: new { userId = registeredUser.Id },
                value: response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseViewModel>> Login(LoginCommand command)
        {
            _logger.LogInformation("Login attempt for user {UserName}", command.UserName);
            var tokenResponse = await Mediator.Send(command);
            var response = new ApiResponse<TokenResponseViewModel>(tokenResponse);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseViewModel>> RefreshToken(RefreshTokenCommand command)
        {
            
            var tokenResponse = await Mediator.Send(command);
            var response = new ApiResponse<TokenResponseViewModel>(tokenResponse);
            return Ok(response);
        }


    }
}
