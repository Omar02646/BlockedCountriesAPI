using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly CountryService _countryService;

        public LogsController(CountryService countryService)
        {
            _countryService = countryService;
        }

      
        [HttpGet("blocked-attempts")]
        public IActionResult GetBlockedAttempts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Page and pageSize must be greater than 0.");

            var result = _countryService.GetBlockedAttempts(page, pageSize);
            return Ok(result);
        }
    }
}