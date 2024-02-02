using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisCacheApi.DataStore;

namespace RedisCacheApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly DataRepository _repository;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        DataRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        var data = await _repository.GetDataAsync<WeatherForecast[]>(1);
        return Ok(data);
    }
}
