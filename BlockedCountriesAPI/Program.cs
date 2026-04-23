using BlockedCountriesAPI.BackgroundServices;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddSingleton<InMemoryRepository>();


builder.Services.AddHttpClient<GeoLocationService>();


builder.Services.AddScoped<CountryService>();


builder.Services.AddHostedService<TemporalBlockCleanupService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();