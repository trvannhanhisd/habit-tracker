using HabitTracker.API.Examples.Command.User;
using HabitTracker.API.Examples.ViewModel;
using HabitTracker.API.Models;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Application.Features.Users.Commands.UpdateUser;
using HabitTracker.Application.Features.Users.Queries.GetUserById;
using HabitTracker.Application.Features.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng.
        /// API này trả về danh sách người dùng cho admin.
        /// </summary>
        /// <returns>Danh sách người dùng</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ListUserViewModelExample))]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Getting all users at {time}", DateTime.Now);
            var users = await Mediator.Send(new GetUsersQuery());
            var response = new ApiResponse<List<UserViewModel>>(users);
            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID.
        /// API này trả về chi tiết một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Thông tin người dùng</returns>
        /// <response code="200">Lấy thông tin thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        [HttpGet("{userId}")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserViewModelExample))]
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

        /// <summary>
        /// Cập nhật thông tin người dùng theo ID.
        /// API này cho phép chỉnh sửa thông tin người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="command">Dữ liệu cập nhật</param>
        /// <returns>Trạng thái cập nhật</returns>
        /// <response code="204">Cập nhật thành công</response>
        /// <response code="400">Yêu cầu không hợp lệ</response>
        [HttpPut("{userId}")]
        [SwaggerRequestExample(typeof(UpdateUserCommand), typeof(UpdateUserCommandExample))]
        [SwaggerResponseExample(StatusCodes.Status204NoContent, typeof(ApiResponse<object>))]
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