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

namespace Weather.Infrastructure.Services
{
    public class AzureWeatherAIService(IHttpClientFactory httpClientFactory, IOptions<WeatherAIServiceOptions> options, ILogger<AzureWeatherAIService> logger) : IWeatherAIService
    {
        private readonly WeatherAIServiceOptions _options = options.Value;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<string> GetWhatToWearAsync(WeatherAIRequest request)
        {
            var messageContent = $"You are an AI assistant that will receive a weather description and give a response on what to wear for that weather in {request.City}, {request.Country}. Weather description: {request.Description}";
            return await SendRequestAsync(messageContent);
        }

        public async Task<string> GetDayRecommendationsAsync(WeatherAIRequest request)
        {
            var messageContent = $"You are an AI assistant that will receive a weather description and provide a description of what a typical day might look like in {request.City}, {request.Country}, along with recommendations for activities. Weather description: {request.Description}";
            return await SendRequestAsync(messageContent);
        }

        private async Task<string> SendRequestAsync(string messageContent)
        {
            var payload = CreatePayload(messageContent);
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            content.Headers.Add("api-key", _options.ApiKey);

            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsync(_options.Endpoint, content);

            HandleRateLimitAsync(response);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            logger.LogInformation("AI Response: {ResponseBody}", responseBody);

            var aiResponse = JsonSerializer.Deserialize<AzureAIResponse>(responseBody, JsonOptions);
            return aiResponse?.Choices.FirstOrDefault()?.Message.Content ?? "No response";
        }

        private static AzureAIPayload CreatePayload(string messageContent)
        {
            return new AzureAIPayload
            {
                Messages =
                [
            new AzureAIMessage
            {
                Role = "system",
                Content = new[]
                {
                    new AzureAIContent
                    {
                        Type = "text",
                        Text = messageContent
                    }
                }
            }
        ],
                Temperature = 0.7,
                TopP = 0.95,
                MaxTokens = 800
            };
        }

        private void HandleRateLimitAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                logger.LogWarning("Azure AI rate limit exceeded. Returning 429.");

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
        }
    }
}
