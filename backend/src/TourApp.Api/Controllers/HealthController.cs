using Microsoft.AspNetCore.Mvc;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<string>> Get()
    {
        return Ok(ApiResponse<string>.Ok("OK"));
    }
}
