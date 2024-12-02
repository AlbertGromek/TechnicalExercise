using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Weather.Application.Interfaces;

namespace Weather.FuncApp
{
    public class WeatherFunctions
    {
        private readonly ILogger<WeatherFunctions> _logger;
        private readonly IWeatherService _openWeatherService;

        public WeatherFunctions(ILogger<WeatherFunctions> logger, IWeatherService openWeatherService)
        {
            _logger = logger;
            _openWeatherService = openWeatherService;
        }

        [Function("GetWeatherForecast")]
        public async Task<IActionResult> GetWeatherForecastAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "forecast")] HttpRequestData req)
        {
            _logger.LogInformation("Processing weather forecast request.");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var city = query["city"];
            var countryCode = query["countryCode"];

            if (string.IsNullOrEmpty(city))
            {
                return new BadRequestObjectResult("City is required.");
            }

            if (string.IsNullOrEmpty(countryCode))
            {
                return new BadRequestObjectResult("Country code is required.");
            }

            var weatherData = await _openWeatherService.GetWeatherDataAsync(city, countryCode);
            if (!string.IsNullOrEmpty(weatherData))
            {
                return new OkObjectResult(weatherData);
            }
            else
            {
                return new BadRequestObjectResult("Unable to retrieve weather data.");
            }
        }
    }
}