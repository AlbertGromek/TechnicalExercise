using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using Weather.Application.Interfaces;

namespace Weather.FuncApp
{
    public class WeatherFunctions(ILogger<WeatherFunctions> logger, IWeatherService openWeatherService)
    {
        private readonly ILogger<WeatherFunctions> logger = logger;
        private readonly IWeatherService openWeatherService = openWeatherService;

        [Function("GetWeatherForecastDescription")]
        [OpenApiOperation(operationId: "GetWeatherForecastDescription", tags: ["Weather"])]
        [OpenApiParameter(name: "city", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The city name")]
        [OpenApiParameter(name: "countryCode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The country code")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Bad Request")]
        public async Task<IActionResult> GetWeatherForecastDescriptionAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "forecast/description")] HttpRequestData req)
        {
            logger.LogInformation("Processing weather forecast description request.");

            var queryParams = req.Query; 
            var city = queryParams["city"];
            var countryCode = queryParams["countryCode"];

            if (string.IsNullOrWhiteSpace(city))
            {
                return new BadRequestObjectResult("City is required.");
            }

            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return new BadRequestObjectResult("Country code is required.");
            }

            var weatherData = await openWeatherService.GetWeatherDataAsync(city, countryCode);

            if (!string.IsNullOrEmpty(weatherData))
            {
                return new OkObjectResult(weatherData);
            }
            else
            {
                return new BadRequestObjectResult("Unable to retrieve weather data.");
            }
        }

        [Function("GetWeatherForecast")]
        public async Task<IActionResult> GetWeatherForecastAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "forecast")] HttpRequestData req)
        {
            logger.LogInformation("Processing weather forecast request.");

            var queryParams = req.Query; 
            var city = queryParams["city"];
            var countryCode = queryParams["countryCode"];

            if (string.IsNullOrWhiteSpace(city))
            {
                return new BadRequestObjectResult("City is required.");
            }

            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return new BadRequestObjectResult("Country code is required.");
            }

            var weatherData = await openWeatherService.GetWeatherDataAsync(city, countryCode);

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
