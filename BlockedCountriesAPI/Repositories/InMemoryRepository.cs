using BlockedCountriesAPI.Models;
using System.Collections.Concurrent;


namespace BlockedCountriesAPI.Repositories
{
    public class InMemoryRepository
    {
        // قاموس لتخزين الدول المحظورة - الـ Key هو كود الدولة زي "US"
        private readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = new();

        // قائمة لتخزين سجلات المحاولات
        private readonly List<BlockAttemptLog> _logs = new();

        // object عشان نأمن الـ logs من مشاكل الـ threading
        private readonly object _logLock = new();

        // ===== Blocked Countries =====

        public bool AddBlockedCountry(BlockedCountry country)
        {
            // TryAdd بترجع false لو الكود موجود بالفعل (منع التكرار)
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
            // lock عشان الـ List مش thread-safe زي الـ ConcurrentDictionary
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
