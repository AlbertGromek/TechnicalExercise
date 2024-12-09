using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using Weather.Application.Configurations;
using Weather.Application.Interfaces;
using Weather.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Net;
using Weather.Domain.Models.AzureAI;

namespace Weather.Infrastructure
{
    public class WeatherAIService : IWeatherAIService
    {
        private readonly WeatherAIServiceOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherAIService> _logger;

        public WeatherAIService(IHttpClientFactory httpClientFactory, IOptions<WeatherAIServiceOptions> options, ILogger<WeatherAIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> GetWhatToWearAsync(WeatherAIRequest request)
        {
            var payload = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = new[]
                        {
                            new
                            {
                                type = "text",
                                text = $"You are an AI assistant that will receive a weather description and give a response on what to wear for that weather in {request.City}, {request.Country}. Weather description: {request.Description}"
                            }
                        }
                    }
                },
                temperature = 0.7,
                top_p = 0.95,
                max_tokens = 800
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            content.Headers.Add("api-key", _options.ApiKey);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(_options.Endpoint, content);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("Azure AI rate limit exceeded. Returning 429.");

                if (response.Headers.TryGetValues("Retry-After", out var retryAfterValues))
                {
                    var retryAfter = retryAfterValues.FirstOrDefault();
                    throw new HttpRequestException($"Rate limit exceeded. Retry after {retryAfter} seconds.", null, HttpStatusCode.TooManyRequests)
                    {
                        Data = { { "Retry-After", retryAfter } }
                    };
                }

                throw new HttpRequestException("Rate limit exceeded by Azure AI.", null, HttpStatusCode.TooManyRequests);
            }
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("AI Response: {ResponseBody}", responseBody);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var aiResponse = JsonSerializer.Deserialize<AzureAIResponse>(responseBody, options);

            return aiResponse?.Choices.FirstOrDefault()?.Message.Content ?? "No response";
        }

        public async Task<string> GetDayRecommendationsAsync(WeatherAIRequest request)
        {
            var payload = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = new[]
                        {
                            new
                            {
                                type = "text",
                                text = $"You are an AI assistant that will receive a weather description and provide a description of what a typical day might look like in {request.City}, {request.Country}, along with recommendations for activities. Weather description: {request.Description}"
                            }
                        }
                    }
                },
                temperature = 0.7,
                top_p = 0.95,
                max_tokens = 800
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            content.Headers.Add("api-key", _options.ApiKey);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(_options.Endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("AI Response: {ResponseBody}", responseBody);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var aiResponse = JsonSerializer.Deserialize<AzureAIResponse>(responseBody, options);

            return aiResponse?.Choices.FirstOrDefault()?.Message.Content ?? "No response";
        }
    }
}