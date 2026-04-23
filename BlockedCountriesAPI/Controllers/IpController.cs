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

    
        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIp([FromQuery] string? ipAddress)
        {
            
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(ipAddress))
                    return BadRequest("Could not determine IP address.");
            }

         
            if (!_geoLocationService.IsValidIpAddress(ipAddress))
                return BadRequest($"Invalid IP address format: {ipAddress}");

            var result = await _geoLocationService.GetIpInfoAsync(ipAddress);

            if (result == null)
                return StatusCode(503, "Could not fetch IP information from geolocation service.");

            return Ok(result);
        }

      
        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress))
                return BadRequest("Could not determine caller IP address.");

          
            var ipInfo = await _geoLocationService.GetIpInfoAsync(ipAddress);

            if (ipInfo == null)
                return StatusCode(503, "Could not fetch IP information.");

      
            var isBlocked = _repository.IsCountryBlocked(ipInfo.Country);

        
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