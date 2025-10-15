using HabitTracker.API.Examples.ViewModel;
using HabitTracker.API.Models;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class HabitLogController : ApiControllerBase
    {

        private readonly ILogger<HabitLogController> _logger;

        public HabitLogController(ILogger<HabitLogController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet("{habitLogId}")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HabitLogViewModelExample))]
        public async Task<IActionResult> GetHabitLogById(int habitLogId)
        {
            _logger.LogInformation("Getting habit log at {time}", DateTime.Now);
            var habitLog = await Mediator.Send(new GetHabitLogByIdQuery() { HabitLogId = habitLogId });
            var response = new ApiResponse<HabitLogViewModel>(habitLog);
            return Ok(response);
        }

    }
}
