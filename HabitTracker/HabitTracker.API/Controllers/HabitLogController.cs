

using HabitTracker.API.Models;
using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogById;
using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs;
using HabitTracker.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.API.Controllers
{
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
        public async Task<IActionResult> GetHabitLogById(int habitLogId)
        {
            _logger.LogInformation("Getting habit log at {time}", DateTime.Now);
            var habitLog = await Mediator.Send(new GetHabitLogByIdQuery() { HabitLogId = habitLogId });
            var response = new ApiResponse<HabitLogViewModel>(habitLog);
            return Ok(response);
        }

    }
}
