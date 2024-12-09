using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Weather.Application.Configurations;
using Weather.Application.Interfaces;
using Weather.Domain.Models.OpenWeatherService;

namespace Weather.Infrastructure
{
    public class OpenWeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenWeatherServiceOptions _options;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public OpenWeatherService(IHttpClientFactory httpClientFactory, IOptions<OpenWeatherServiceOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

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

            var client = _httpClientFactory.CreateClient();

            using var response = await client.GetAsync(requestUri);

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