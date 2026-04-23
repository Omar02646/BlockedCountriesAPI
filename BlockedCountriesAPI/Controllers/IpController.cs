using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/ip")]
    public class IpController : ControllerBase
    {
        private readonly GeoLocationService _geoLocationService;
        private readonly InMemoryRepository _repository;

        public IpController(GeoLocationService geoLocationService, InMemoryRepository repository)
        {
            _geoLocationService = geoLocationService;
            _repository = repository;
        }

        // 4. البحث عن بيانات IP معين
        // GET /api/ip/lookup?ipAddress=8.8.8.8
        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIp([FromQuery] string? ipAddress)
        {
            // لو مفيش IP في الـ query، جيب IP المتصل نفسه
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(ipAddress))
                    return BadRequest("Could not determine IP address.");
            }

            // تحقق إن الـ IP صيغته صح
            if (!_geoLocationService.IsValidIpAddress(ipAddress))
                return BadRequest($"Invalid IP address format: {ipAddress}");

            var result = await _geoLocationService.GetIpInfoAsync(ipAddress);

            if (result == null)
                return StatusCode(503, "Could not fetch IP information from geolocation service.");

            return Ok(result);
        }

        // 5. تحقق لو IP المتصل محظور
        // GET /api/ip/check-block
        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            // 1. جيب IP المتصل
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress))
                return BadRequest("Could not determine caller IP address.");

            // 2. جيب بيانات الدولة من ipapi.co
            var ipInfo = await _geoLocationService.GetIpInfoAsync(ipAddress);

            if (ipInfo == null)
                return StatusCode(503, "Could not fetch IP information.");

            // 3. تحقق لو الدولة في القائمة المحظورة
            var isBlocked = _repository.IsCountryBlocked(ipInfo.Country);

            // 4. سجّل المحاولة في اللوج
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            _repository.AddLog(new BlockAttemptLog
            {
                IpAddress = ipAddress,
                CountryCode = ipInfo.Country,
                CountryName = ipInfo.Country_Name,
                IsBlocked = isBlocked,
                Timestamp = DateTime.UtcNow,
                UserAgent = userAgent
            });

            return Ok(new
            {
                IpAddress = ipAddress,
                CountryCode = ipInfo.Country,
                CountryName = ipInfo.Country_Name,
                IsBlocked = isBlocked,
                Message = isBlocked
                    ? $"Access denied. Your country {ipInfo.Country_Name} is blocked."
                    : $"Access allowed. Your country {ipInfo.Country_Name} is not blocked."
            });
        }
    }
}