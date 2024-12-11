using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using Weather.Application.Configurations;
using Weather.Domain.Models;
using Weather.Infrastructure.Services;

namespace Weather.Infrastructure.Tests
{
    public class WeatherAIServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<ILogger<WeatherAIService>> _loggerMock;
        private readonly WeatherAIServiceOptions _options;
        private readonly WeatherAIService _weatherAIService;

        public WeatherAIServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<WeatherAIService>>();
            _options = new WeatherAIServiceOptions
            {
                ApiKey = "test-api-key",
                Endpoint = "https://test-endpoint"
            };

            _weatherAIService = new WeatherAIService(_httpClientFactoryMock.Object, Options.Create(_options), _loggerMock.Object);
        }

        [Fact]
        public async Task GetWhatToWearAsync_ShouldReturnResponseContent()
        {
            // Arrange
            var request = new WeatherAIRequest { Description = "Sunny", City = "Melbourne", Country = "AU" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"choices\":[{\"message\":{\"content\":\"Wear sunglasses\"}}]}")
            };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var client = new HttpClient(httpClientMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = await _weatherAIService.GetWhatToWearAsync(request);

            // Assert
            Assert.Equal("Wear sunglasses", result);
        }

        [Theory]
        [InlineData(60)]
        [InlineData(30)]
        [InlineData(10)]
        public async Task GetWhatToWearAsync_ShouldThrowHttpRequestException_WhenRateLimitExceeded(int retryAfterSeconds)
        {
            // Arrange
            var request = new WeatherAIRequest { Description = "Sunny", City = "Melbourne", Country = "AU" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent($"Rate limit exceeded. Retry after {retryAfterSeconds} seconds."),
                Headers = { { "Retry-After", retryAfterSeconds.ToString() } }
            };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var client = new HttpClient(httpClientMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _weatherAIService.GetWhatToWearAsync(request));
            Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
            Assert.Equal($"Rate limit exceeded. Retry after {retryAfterSeconds} seconds.", exception.Message);
        }

        [Fact]
        public async Task GetDayRecommendationsAsync_ShouldReturnResponseContent()
        {
            // Arrange
            var request = new WeatherAIRequest { Description = "Sunny", City = "Melbourne", Country = "AU" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"choices\":[{\"message\":{\"content\":\"Go for a walk\"}}]}")
            };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var client = new HttpClient(httpClientMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = await _weatherAIService.GetDayRecommendationsAsync(request);

            // Assert
            Assert.Equal("Go for a walk", result);
        }

        [Theory]
        [InlineData(60, "Rate limit exceeded. Please try again later.")]
        [InlineData(45, "Rate limit exceeded. Please try again later.")]
        [InlineData(15, "Rate limit exceeded. Please try again later.")]
        public async Task GetDayRecommendationsAsync_ShouldThrowHttpRequestException_WhenRateLimitExceeded(int retryAfterSeconds, string responseContent)
        {
            // Arrange
            var request = new WeatherAIRequest { Description = "Sunny", City = "Melbourne", Country = "AU" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent(responseContent),
                Headers = { { "Retry-After", retryAfterSeconds.ToString() } }
            };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var client = new HttpClient(httpClientMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _weatherAIService.GetDayRecommendationsAsync(request));
            Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
            Assert.Contains($"Rate limit exceeded. Retry after {retryAfterSeconds} seconds.", exception.Message);
        }
    }
}