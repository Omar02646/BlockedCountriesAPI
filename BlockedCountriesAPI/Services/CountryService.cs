using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories;

namespace BlockedCountriesAPI.Services
{
    public class CountryService
    {
        private readonly InMemoryRepository _repository;
        private readonly GeoLocationService _geoLocationService;

        public CountryService(InMemoryRepository repository, GeoLocationService geoLocationService)
        {
            _repository = repository;
            _geoLocationService = geoLocationService;
        }

        // إضافة دولة للحظر
        public bool BlockCountry(string countryCode, string countryName)
        {
            var country = new BlockedCountry
            {
                CountryCode = countryCode.ToUpper(),
                CountryName = countryName,
                BlockedAt = DateTime.UtcNow,
                IsTemporary = false
            };
            return _repository.AddBlockedCountry(country);
        }

        // حظر مؤقت
        public bool TemporalBlockCountry(string countryCode, string countryName, int durationMinutes)
        {
            var country = new BlockedCountry
            {
                CountryCode = countryCode.ToUpper(),
                CountryName = countryName,
                BlockedAt = DateTime.UtcNow,
                IsTemporary = true,
                ExpiresAt = DateTime.UtcNow.AddMinutes(durationMinutes)
            };
            return _repository.AddBlockedCountry(country);
        }

        // جيب كل الدول المحظورة مع Pagination و Search
        public object GetBlockedCountries(int page, int pageSize, string? search)
        {
            var all = _repository.GetAllBlockedCountries();

            // فلترة لو فيه search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToUpper();
                all = all.Where(c =>
                    c.CountryCode.Contains(search) ||
                    c.CountryName.ToUpper().Contains(search));
            }

            var total = all.Count();

            // Pagination
            var items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        // جيب سجلات المحاولات مع Pagination
        public object GetBlockedAttempts(int page, int pageSize)
        {
            var all = _repository.GetLogs();

            // ✅ أضف الفلترة دي عشان ترجع المحاولات المرفوضة بس
            var blockedOnly = all.Where(l => l.IsBlocked);

            var total = blockedOnly.Count();

            var items = blockedOnly
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

    }
}
