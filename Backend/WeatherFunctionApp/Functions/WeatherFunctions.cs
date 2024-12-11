using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using Weather.Application.Interfaces;
using Weather.Domain.Models;

namespace Weather.FuncApp.Functions
{
    public class WeatherFunctions(ILogger<WeatherFunctions> logger, IWeatherService openWeatherService, IWeatherAIService weatherAIService)
    {
        private readonly ILogger<WeatherFunctions> _logger = logger;
        private readonly IWeatherService _openWeatherService = openWeatherService;
        private readonly IWeatherAIService _weatherAIService = weatherAIService;

        [Function("GetWeatherForecastDescription")]
        [OpenApiOperation(operationId: "GetWeatherForecastDescription", tags: ["Weather"])]
        [OpenApiRequestBody("application/json", typeof(WeatherDescriptionRequest), Description = "The weather description request", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Bad Request")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "City or country not found")]
        public async Task<IActionResult> GetWeatherForecastDescriptionAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "forecast/description")] HttpRequestData req)
        {
            _logger.LogInformation("Processing weather forecast description request.");

            var request = await req.ReadFromJsonAsync<WeatherDescriptionRequest>();

            if (request == null || string.IsNullOrWhiteSpace(request.City) || string.IsNullOrWhiteSpace(request.CountryCode))
            {
                return new BadRequestObjectResult("Invalid request. City and CountryCode are required.");
            }

            try
            {
                var weatherData = await _openWeatherService.GetWeatherDataAsync(request.City, request.CountryCode);

                if (!string.IsNullOrEmpty(weatherData))
                {
                    return new OkObjectResult(weatherData);
                }
                else
                {
                    return new BadRequestObjectResult("Unable to retrieve weather data.");
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new NotFoundObjectResult("City or country not found");
            }
        }

        [Function("GetWhatToWear")]
        [OpenApiOperation(operationId: "GetWhatToWear", tags: ["AI"])]
        [OpenApiRequestBody("application/json", typeof(WeatherAIRequest), Description = "The weather AI request", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(WeatherAIResponse), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.TooManyRequests, contentType: "application/json", bodyType: typeof(WeatherAIResponse), Description = "Rate limit exceeded")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(WeatherAIResponse), Description = "Bad Request")]
        public async Task<HttpResponseData> GetWhatToWearAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "ai/what-to-wear")] HttpRequestData req)
        {
            _logger.LogInformation("Processing AI what to wear request.");

            var request = await req.ReadFromJsonAsync<WeatherAIRequest>();

            if (request == null || string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.City) || string.IsNullOrWhiteSpace(request.Country))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                var errorResponse = new WeatherAIResponse("Invalid request. Description, City, and Country are required.");
                await badRequestResponse.WriteAsJsonAsync(errorResponse);
                return badRequestResponse;
            }

            try
            {
                var aiResponse = await _weatherAIService.GetWhatToWearAsync(request);

                var okResponse = req.CreateResponse(HttpStatusCode.OK);
                var successResponse = new WeatherAIResponse(aiResponse);
                await okResponse.WriteAsJsonAsync(successResponse);
                return okResponse;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return await HandleTooManyRequestsAsync(req, ex);
            }
        }


        [Function("GetDayRecommendations")]
        [OpenApiOperation(operationId: "GetDayRecommendations", tags: ["AI"])]
        [OpenApiRequestBody("application/json", typeof(WeatherAIRequest), Description = "The weather AI request", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(WeatherAIResponse), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.TooManyRequests, contentType: "application/json", bodyType: typeof(WeatherAIResponse), Description = "Rate limit exceeded")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(WeatherAIResponse), Description = "Bad Request")]
        public async Task<HttpResponseData> GetDayRecommendationsAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "ai/day-recommendations")] HttpRequestData req)
        {
            _logger.LogInformation("Processing AI day recommendations request.");

            var request = await req.ReadFromJsonAsync<WeatherAIRequest>();

            if (request == null || string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.City) || string.IsNullOrWhiteSpace(request.Country))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                var errorResponse = new WeatherAIResponse("Invalid request. Description, City, and Country are required.");
                await badRequestResponse.WriteAsJsonAsync(errorResponse);
                return badRequestResponse;
            }

            try
            {
                var aiResponse = await _weatherAIService.GetDayRecommendationsAsync(request);

                var okResponse = req.CreateResponse(HttpStatusCode.OK);
                var successResponse = new WeatherAIResponse(aiResponse);
                await okResponse.WriteAsJsonAsync(successResponse);
                return okResponse;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return await HandleTooManyRequestsAsync(req, ex);
            }
        }

        private static async Task<HttpResponseData> HandleTooManyRequestsAsync(HttpRequestData req, HttpRequestException ex)
        {
            var retryAfter = ex.Data["Retry-After"]?.ToString();

            var tooManyRequestsResponse = req.CreateResponse(HttpStatusCode.TooManyRequests);
            var errorContent = retryAfter != null
                ? $"Rate limit exceeded. Please retry later. Retry-After: {retryAfter}"
                : "Rate limit exceeded. Please retry later.";
            var errorResponse = new WeatherAIResponse(errorContent);

            await tooManyRequestsResponse.WriteAsJsonAsync(errorResponse);
            return tooManyRequestsResponse;
        }
    }
}
