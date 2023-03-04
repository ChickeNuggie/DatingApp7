using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// [ApiController] //API route
// [Route("[controller]")] //API route

// Inherit fron BaseApiController
public class WeatherForecastController : BaseApiController //MVC , Model and Controller from entity framework. View supplied by angular application
//Older version: View = html returned based on that model, controller does logic and the processing that gives us the data inside the model which is then presented in the view.
// 
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get() // return list of type weatherforecast class
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            // Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            Summary = "Hello World"
        })
        .ToArray();
    }
}
