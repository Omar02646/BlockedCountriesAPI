using BlockedCountriesAPI.Models;
using Newtonsoft.Json;
using System.Net;

namespace BlockedCountriesAPI.Services
{
    public class GeoLocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeoLocationService> _logger;

        public GeoLocationService(HttpClient httpClient, ILogger<GeoLocationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IpApiResponse?> GetIpInfoAsync(string ipAddress)
        {
            try
            {
               
                if (ipAddress == "::1" || ipAddress == "127.0.0.1")
                {
                    var ipResponse = await _httpClient.GetStringAsync("https://api.ipify.org");
                    ipAddress = ipResponse.Trim();
                    _logger.LogInformation($"Localhost detected, using public IP: {ipAddress}");
                }

       
                string url = $"http://ip-api.com/json/{ipAddress}?fields=66846719";
               

                _logger.LogInformation($"Calling: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API returned {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IpApiResponse>(json);

                if (result == null || string.IsNullOrEmpty(result.Country))
                {
                    _logger.LogError("Failed to parse response");
                    return null;
                }

                _logger.LogInformation($"Success: {result.Country} - {result.Country_Name}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return null;
            }
        }

        public bool IsValidIpAddress(string ipAddress)
        {
            return IPAddress.TryParse(ipAddress, out _);
        }
    }
}