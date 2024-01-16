using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisCacheApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private const string CACHE_KEY = "WeatherForecast";
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IDistributedCache _cache;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        // check if data is in cache
        var cachedData = await _cache.GetStringAsync(CACHE_KEY);
        if (cachedData != null)
        {
            _logger.LogInformation("Obtained from cache");
            var result = JsonSerializer.Deserialize<WeatherForecast[]>(cachedData);
            return Ok(result);
        }

        var cast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        // Cache data for 5 minutes
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
        };

        // Store data in cache
        var serializedData = JsonSerializer.Serialize(cast);
        await _cache.SetStringAsync(CACHE_KEY, serializedData, options);

        return Ok(cast);
    }
}
