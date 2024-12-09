using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using Weather.Application.Configurations;

namespace Weather.Infrastructure.Tests
{
    public class OpenWeatherServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly OpenWeatherServiceOptions _options;
        private readonly OpenWeatherService _openWeatherService;

        public OpenWeatherServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _options = new OpenWeatherServiceOptions
            {
                ApiKey = "test-api-key",
                WeatherForecastURL = "http://api.openweathermap.org/data/2.5/weather"
            };

            _openWeatherService = new OpenWeatherService(_httpClientFactoryMock.Object, Options.Create(_options));
        }

        [Fact]
        public async Task GetWeatherDataAsync_ShouldReturnWeatherDescription()
        {
            // Arrange
            var city = "Melbourne";
            var countryCode = "AU";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"weather\":[{\"description\":\"Sunny\"}]}")
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
            var result = await _openWeatherService.GetWeatherDataAsync(city, countryCode);

            // Assert
            Assert.Equal("Sunny", result);
        }

        [Fact]
        public async Task GetWeatherDataAsync_ShouldThrowArgumentException_WhenCityIsEmpty()
        {
            // Arrange
            var city = "";
            var countryCode = "AU";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _openWeatherService.GetWeatherDataAsync(city, countryCode));
            Assert.Equal("City name must be provided (Parameter 'city')", exception.Message);
        }

        [Fact]
        public async Task GetWeatherDataAsync_ShouldThrowException_WhenStatusCodeIsNotSuccess()
        {
            // Arrange
            var city = "Melbourne";
            var countryCode = "AU";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
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
            await Assert.ThrowsAsync<HttpRequestException>(() => _openWeatherService.GetWeatherDataAsync(city, countryCode));
        }
    }
}