using HabitTracker.API.Examples.Command.Habit;
using HabitTracker.API.Examples.ViewModel;
using HabitTracker.API.Models;
using HabitTracker.Application.Common.ViewModels;
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
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Controllers
{
    [ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class HabitController : ApiControllerBase
    {
        private readonly ILogger<HabitController> _logger;

        public HabitController(ILogger<HabitController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả thói quen.
        /// API này trả về danh sách thói quen cho admin.
        /// </summary>
        /// <returns>Danh sách thói quen</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ListHabitViewModelExample))]
        [ProducesResponseType(typeof(ApiResponse<List<HabitViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        //[MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAllHabits()
        {
            _logger.LogInformation("Getting all habits at {time}", DateTime.Now);
            var habits = await Mediator.Send(new GetHabitQuery());
            var response = new ApiResponse<List<HabitViewModel>>(habits);
            return Ok(response);
        }


        //[MapToApiVersion("2.0")]
        //[HttpGet]
        //public async Task<IActionResult> GetAllHabitsV2()
        //{
        //    _logger.LogInformation("Getting all habits at {time}", DateTime.Now);
        //    var habits = await Mediator.Send(new GetHabitQuery());
        //    var response = new ApiResponse<List<HabitViewModel>>(habits);
        //    return Ok(response);
        //}


        /// <summary>
        /// Lấy thông tin thói quen theo ID.
        /// API này trả về chi tiết một thói quen.
        /// </summary>
        /// <param name="habitId">ID của thói quen</param>
        /// <returns>Thông tin thói quen</returns>
        /// <response code="200">Lấy thông tin thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        [Authorize]
        [HttpGet("{habitId}")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HabitViewModelExample))]
        [ProducesResponseType(typeof(ApiResponse<HabitViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetHabitById(int habitId)
        {
            _logger.LogInformation("Getting habit at {time}", DateTime.Now);
            var habit = await Mediator.Send(new GetHabitByIdQuery() { HabitId = habitId });
            var response = new ApiResponse<HabitViewModel>(habit);
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách thói quen của người dùng.
        /// API này trả về thói quen thuộc về người dùng hiện tại.
        /// </summary>
        /// <returns>Danh sách thói quen của người dùng</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        [Authorize(Roles = "User")]
        [HttpGet("user")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ListHabitViewModelExample))]
        [ProducesResponseType(typeof(ApiResponse<List<HabitViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetHabitsByUser()
        {
            _logger.LogInformation("Getting all habits of user at {time}", DateTime.Now);
            var userId = User.GetUserId();
            if (userId == 0)
            {
                _logger.LogWarning("Unauthorized attempt to get habits for user {userId} done", userId);
                var errorResponse = new ApiResponse<object>("Unauthorized attempt to get habits for user done", 401);
                return Unauthorized(errorResponse);
            }

            var habits = await Mediator.Send(new GetHabitsByUserQuery { UserId = userId });
            var response = new ApiResponse<List<HabitViewModel>>(habits);
            return Ok(response);
        }

        /// <summary>
        /// Tạo một thói quen mới.
        /// API này cho phép người dùng tạo thói quen mới.
        /// </summary>
        /// <param name="command">Dữ liệu thói quen cần tạo</param>
        /// <returns>Thông tin thói quen đã tạo</returns>
        /// <response code="201">Tạo thói quen thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        [Authorize(Roles = "User")]
        [HttpPost]
        [SwaggerRequestExample(typeof(CreateHabitCommand), typeof(CreateHabitCommandExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(HabitViewModelExample))]
        [ProducesResponseType(typeof(ApiResponse<HabitViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateHabit(CreateHabitCommand command)
        {
            _logger.LogInformation("Creating habit at {time}", DateTime.Now);
            var createdHabit = await Mediator.Send(command);
            var response = new ApiResponse<HabitViewModel>(createdHabit, 201);
            return CreatedAtAction(nameof(GetHabitById), new { habitId = createdHabit.Id }, response);
        }

        /// <summary>
        /// Đánh dấu thói quen đã hoàn thành.
        /// API này ghi nhận thói quen được hoàn thành.
        /// </summary>
        /// <param name="habitId">ID của thói quen</param>
        /// <returns>Thông tin log hoàn thành</returns>
        /// <response code="201">Đánh dấu thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        [Authorize(Roles = "User")]
        [HttpPost("{habitId}/done")]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(HabitLogViewModelExample))]
        [ProducesResponseType(typeof(ApiResponse<HabitLogViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Cập nhật thói quen theo ID.
        /// API này cho phép chỉnh sửa thông tin thói quen.
        /// </summary>
        /// <param name="habitId">ID của thói quen</param>
        /// <param name="command">Dữ liệu cập nhật</param>
        /// <returns>Trạng thái cập nhật</returns>
        /// <response code="204">Cập nhật thành công</response>
        /// <response code="400">Yêu cầu không hợp lệ</response>
        /// <response code="401">Không có quyền truy cập</response>
        [Authorize]
        [HttpPut("{habitId}")]
        [SwaggerRequestExample(typeof(UpdateHabitCommand), typeof(UpdateHabitCommandExample))]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateHabit(int habitId, UpdateHabitCommand command)
        {
            _logger.LogInformation("Updating habit at {time}", DateTime.Now);
            if (habitId != command.Id)
            {
                var errorResponse = new ApiResponse<object>("Habit with ID not found.", 400);
                return BadRequest(errorResponse);
            }
            await Mediator.Send(command);

            return Ok(new ApiResponse<object>(null, 204));
        }

        /// <summary>
        /// Lưu trữ (archived) thói quen theo ID.
        /// API này di chuyển thói quen sang trạng thái lưu trữ.
        /// </summary>
        /// <param name="habitId">ID của thói quen</param>
        /// <returns>Trạng thái lưu trữ</returns>
        /// <response code="204">Lưu trữ thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        [Authorize(Roles = "User")]
        [HttpPatch("{habitId}/archive")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ArchiveHabit(int habitId)
        {
            _logger.LogInformation("Archiving habit at {time}", DateTime.Now);
            await Mediator.Send(new ArchiveHabitCommand { HabitId = habitId });
            return Ok(new ApiResponse<object>(null, 204));
        }

        /// <summary>
        /// Xóa thói quen theo ID.
        /// API này xóa vĩnh viễn một thói quen.
        /// </summary>
        /// <param name="habitId">ID của thói quen</param>
        /// <returns>Trạng thái xóa</returns>
        /// <response code="204">Xóa thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        [Authorize]
        [HttpDelete("{habitId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteHabit(int habitId)
        {
            _logger.LogInformation("Deleting habit {HabitId} at {Time}", habitId, DateTime.UtcNow);
            await Mediator.Send(new DeleteHabitCommand(habitId));
            return Ok(new ApiResponse<object>(null, 204));
        }

        /// <summary>
        /// Lấy danh sách log của thói quen theo ID.
        /// API này trả về lịch sử hoàn thành thói quen.
        /// </summary>
        /// <param name="habitId">ID của thói quen</param>
        /// <returns>Danh sách log thói quen</returns>
        /// <response code="200">Lấy log thành công</response>
        [Authorize]
        [HttpGet("{habitId}/logs")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HabitLogViewModelExample))]
        public async Task<IActionResult> GetHabitLogsByHabit(int habitId)
        {
            _logger.LogInformation("Getting habit log at {time}", DateTime.Now);
            var query = new GetHabitLogsByHabitQuery
            {
                HabitId = habitId,
            };
            var habitLogList = await Mediator.Send(query);
            var response = new ApiResponse<List<HabitLogViewModel>>(habitLogList);
            return Ok(response);
        }
    }
}