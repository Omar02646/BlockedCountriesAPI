using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly CountryService _countryService;
        private readonly InMemoryRepository _repository;

        public CountriesController(CountryService countryService, InMemoryRepository repository)
        {
            _countryService = countryService;
            _repository = repository;
        }

        // 1. إضافة دولة للحظر
        // POST /api/countries/block
        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] BlockCountryRequest request)
        {
            if (string.IsNullOrEmpty(request.CountryCode) || request.CountryCode.Length != 2)
                return BadRequest("Country code must be 2 letters (e.g. US, EG)");

            var added = _countryService.BlockCountry(request.CountryCode, request.CountryName ?? "");

            if (!added)
                return Conflict($"Country {request.CountryCode} is already blocked.");

            return Ok($"Country {request.CountryCode} has been blocked.");
        }

        // 2. حذف دولة من الحظر
        // DELETE /api/countries/block/{countryCode}
        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            var removed = _repository.RemoveBlockedCountry(countryCode);

            if (!removed)
                return NotFound($"Country {countryCode} is not in the blocked list.");

            return Ok($"Country {countryCode} has been unblocked.");
        }

        // 3. جيب كل الدول المحظورة مع Pagination و Search
        // GET /api/countries/blocked?page=1&pageSize=10&search=US
        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Page and pageSize must be greater than 0.");

            var result = _countryService.GetBlockedCountries(page, pageSize, search);
            return Ok(result);
        }

        // 7. حظر مؤقت
        // POST /api/countries/temporal-block
        [HttpPost("temporal-block")]
        public IActionResult TemporalBlock([FromBody] TemporalBlockRequest request)
        {
            // Validation
            if (string.IsNullOrEmpty(request.CountryCode) || request.CountryCode.Length != 2)
                return BadRequest("Country code must be 2 letters.");

            if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
                return BadRequest("Duration must be between 1 and 1440 minutes.");

            // تحقق لو الدولة محظورة بالفعل
            if (_repository.IsCountryBlocked(request.CountryCode))
                return Conflict($"Country {request.CountryCode} is already blocked.");

            var added = _countryService.TemporalBlockCountry(
                request.CountryCode,
                request.CountryName ?? "",
                request.DurationMinutes);

            if (!added)
                return Conflict($"Country {request.CountryCode} is already temporarily blocked.");

            return Ok(new
            {
                Message = $"Country {request.CountryCode} blocked temporarily.",
                ExpiresAt = DateTime.UtcNow.AddMinutes(request.DurationMinutes)
            });
        }
    }

    // Request Models
    public class BlockCountryRequest
    {
        public string CountryCode { get; set; } = string.Empty;
        public string? CountryName { get; set; }
    }

    public class TemporalBlockRequest
    {
        public string CountryCode { get; set; } = string.Empty;
        public string? CountryName { get; set; }
        public int DurationMinutes { get; set; }
    }
}