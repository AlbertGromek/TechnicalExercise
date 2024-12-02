using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using Weather.Application.Configurations;
using Weather.Application.Interfaces;
using Weather.Domain.Models;

namespace WeatherData.Infastructure
{
    public class OpenWeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenWeatherServiceOptions _options;

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

            var uriBuilder = new UriBuilder(_options.WeatherForecastURL);
            var queryParams = new Dictionary<string, string>
            {
                ["appid"] = _options.ApiKey,
                ["q"] = location
            };
            uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var client = _httpClientFactory.CreateClient();

            try
            {
                using var response = await client.GetAsync(uriBuilder.Uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return weatherForecast?.Weather?.FirstOrDefault()?.Description ?? "No weather description available.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        var errorDetails = JsonSerializer.Deserialize<WeatherErrorObject>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return errorDetails?.Message ?? "Error fetching weather data.";
                    }

                    throw new HttpRequestException($"Error fetching weather data: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching weather data.", ex);
            }
        }
    }
}
