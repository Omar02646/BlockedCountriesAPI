using BlockedCountriesAPI.Models;
using System.Collections.Concurrent;


namespace BlockedCountriesAPI.Repositories
{
    public class InMemoryRepository
    {
        
        private readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = new();

    
        private readonly List<BlockAttemptLog> _logs = new();

        private readonly object _logLock = new();

        // ===== Blocked Countries =====

        public bool AddBlockedCountry(BlockedCountry country)
        {
           
            return _blockedCountries.TryAdd(country.CountryCode.ToUpper(), country);
        }

        public bool RemoveBlockedCountry(string countryCode)
        {
            return _blockedCountries.TryRemove(countryCode.ToUpper(), out _);
        }

        public bool IsCountryBlocked(string countryCode)
        {
            return _blockedCountries.ContainsKey(countryCode.ToUpper());
        }

        public IEnumerable<BlockedCountry> GetAllBlockedCountries()
        {
            return _blockedCountries.Values;
        }

        public BlockedCountry? GetBlockedCountry(string countryCode)
        {
            _blockedCountries.TryGetValue(countryCode.ToUpper(), out var country);
            return country;
        }

        // ===== Logs =====

        public void AddLog(BlockAttemptLog log)
        {
    
            lock (_logLock)
            {
                _logs.Add(log);
            }
        }

        public IEnumerable<BlockAttemptLog> GetLogs()
        {
            lock (_logLock)
            {
                return _logs.ToList();
            }
        }

    }
}
