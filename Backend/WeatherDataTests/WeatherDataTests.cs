using System.Threading.Tasks;
using Xunit;
using Moq;
using WeatherData;
using WeatherData.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using WeatherData.Models;

namespace WeatherDataTests
{
    public class WeatherDataTests
    {
        private readonly Mock<IWeatherService> _weatherServiceMock = new Mock<IWeatherService>();
        
        private string validResponseFromWeatherService;
        private string invalidResponseFromWeatherService;
        public WeatherDataTests()
        {
            validResponseFromWeatherService = "partly cloudy";
            var errorObject = new WeatherErrorObject
            {
                Cod = "404",
                Message = "city not found",
            };
            invalidResponseFromWeatherService = JsonSerializer.Serialize(errorObject);
            _weatherServiceMock.Setup(mock => mock.GetWeatherData(It.Is<string>(s => s.Contains("asdasd")), It.Is<string>(s => s.Contains("asdasd")))).ReturnsAsync(invalidResponseFromWeatherService);
            _weatherServiceMock.Setup(mock => mock.GetWeatherData(It.Is<string>(s => s.Contains("Melbourne")), It.Is<string>(s => s.Contains("AU")))).ReturnsAsync(validResponseFromWeatherService);
        }

        [Fact]
        public async Task GetWeatherReport_WhenCityandCountryCodeAreValid_ReturnsWeatherDescription()
        {
            //Arrange
            
            var controller = new WeatherForecastController(_weatherServiceMock.Object);
            //Act
            var actionResult = await controller.GetForecastAsync("Melbourne", "AU");
            var result = actionResult as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result.Value);
            Assert.Equal(validResponseFromWeatherService, result.Value);
        }

        [Fact]
        public async Task GetWeatherReport_WhenCityandCountryCodeAreInValid_ReturnsErrorObject()
        {
            //Arrange

            var controller = new WeatherForecastController(_weatherServiceMock.Object);

            //Act
            var actionResult = await controller.GetForecastAsync("asdasd","asdasd");
            var result = actionResult as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result.Value);
            Assert.Equal(invalidResponseFromWeatherService, result.Value);
        }

        [Fact]
        public async Task TestAPIKeyMiddlewareWithCorrectAPIKey_ExpectedResponse_NotFound()
        {
            using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddJsonFile("appsettings.json");
                    })
                    .ConfigureServices(services =>
                    {

                    })
                    .Configure(app =>
                    {
                        app.UseMiddleware<ApiKeyMiddleware>();
                    });
            })
            .StartAsync();

            var client = host.GetTestClient();
            client.DefaultRequestHeaders.Add("ClientID", "Bob");
            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestAPIKeyMiddlewareWithInCorrectAPIKey_ExpectedResponse_NotAuthorized()
        {
            using var host = await new HostBuilder().ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddJsonFile("appsettings.json");
                    })
                    .ConfigureServices(services =>
                    {

                    })
                    .Configure(app =>
                    {
                        app.UseMiddleware<ApiKeyMiddleware>();
                    });
            })
            .StartAsync();

            var client = host.GetTestClient();
            client.DefaultRequestHeaders.Add("ClientID", "AFakeKey");
            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
