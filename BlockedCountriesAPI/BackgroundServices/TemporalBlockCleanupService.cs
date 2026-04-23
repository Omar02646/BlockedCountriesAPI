using BlockedCountriesAPI.Repositories;
namespace BlockedCountriesAPI.BackgroundServices
{
   

        public class TemporalBlockCleanupService : BackgroundService
        {
            private readonly InMemoryRepository _repository;
            private readonly ILogger<TemporalBlockCleanupService> _logger;

            // كل قد ايه بيشتغل — 5 دقايق
            private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

            public TemporalBlockCleanupService(
                InMemoryRepository repository,
                ILogger<TemporalBlockCleanupService> logger)
            {
                _repository = repository;
                _logger = logger;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                _logger.LogInformation("Cleanup Service started.");

                // بيشتغل طول ما البرنامج شغال
                while (!stoppingToken.IsCancellationRequested)
                {
                    CleanupExpiredBlocks();

                    // استنى 5 دقايق وبعدين اشتغل تاني
                    await Task.Delay(_interval, stoppingToken);
                }
            }

            private void CleanupExpiredBlocks()
            {
                // جيب كل الدول المحظورة مؤقتاً واللي انتهت مدتها
                var expiredCountries = _repository
                    .GetAllBlockedCountries()
                    .Where(c => c.IsTemporary &&
                                c.ExpiresAt.HasValue &&
                                c.ExpiresAt.Value <= DateTime.UtcNow)
                    .ToList();

                foreach (var country in expiredCountries)
                {
                    _repository.RemoveBlockedCountry(country.CountryCode);
                    _logger.LogInformation(
                        $"Temporal block expired and removed: {country.CountryCode}");
                }

                if (expiredCountries.Any())
                    _logger.LogInformation(
                        $"Cleanup done: removed {expiredCountries.Count} expired blocks.");
            }


        }
    }
