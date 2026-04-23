using Newtonsoft.Json;

namespace BlockedCountriesAPI.Models
{
    public class IpApiResponse
    {
        [JsonProperty("ip")]
        public string Ip { get; set; } = string.Empty;

        [JsonProperty("countryCode")]
        public string Country { get; set; } = string.Empty;

        [JsonProperty("country")]
        public string Country_Name { get; set; } = string.Empty;

        [JsonProperty("isp")]
        public string Org { get; set; } = string.Empty;

        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;

        [JsonProperty("regionName")]
        public string Region { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
    }
}