using BlockedCountriesAPI.BackgroundServices;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== تسجيل الـ Services =====

// Repository — Singleton
builder.Services.AddSingleton<InMemoryRepository>();

// GeoLocationService — مع HttpClient
builder.Services.AddHttpClient<GeoLocationService>();

// CountryService
builder.Services.AddScoped<CountryService>();

// Background Service
builder.Services.AddHostedService<TemporalBlockCleanupService>();

// ===== Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();