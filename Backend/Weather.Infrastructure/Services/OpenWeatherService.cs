using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Weather.Application.Configurations;
using Weather.Application.Interfaces;
using Weather.Domain.Models.OpenWeatherService;

namespace Weather.Infrastructure.Services
{
    public class OpenWeatherService(IHttpClientFactory httpClientFactory, IOptions<OpenWeatherServiceOptions> options) : IWeatherService
    {
        private readonly OpenWeatherServiceOptions _options = options.Value;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<string> GetWeatherDataAsync(string city, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("City name must be provided", nameof(city));
            }

            var location = string.IsNullOrEmpty(countryCode) ? city : $"{city},{countryCode}";

            var queryParams = new Dictionary<string, string?>
            {
                ["appid"] = _options.ApiKey,
                ["q"] = location
            };

            var requestUri = QueryHelpers.AddQueryString(_options.WeatherForecastURL, queryParams);

            var client = httpClientFactory.CreateClient();

            using var response = await client.GetAsync(requestUri);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new HttpRequestException($"City/Country combination was not found", null, System.Net.HttpStatusCode.NotFound);

            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to OpenWeatherMap API failed with status code {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(content, _jsonSerializerOptions);

            return weatherForecast?.Weather?.FirstOrDefault()?.Description ?? "No weather description available.";
        }
    }
}