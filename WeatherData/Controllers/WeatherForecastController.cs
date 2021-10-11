using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;

namespace WeatherData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        [Route("forecast")]
        [HttpGet]
        public async Task<WeatherForecast> GetForecastAsync(string city, string countryCode)
        {
            var apiKey = _configuration.GetValue<string>("APIKey");
            var url = _configuration.GetValue<string>("WeatherForecastURL");
            var client = _clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var query = new Dictionary<string, string>
            {
                ["appid"] = apiKey,
                ["q"] = $"{city},{countryCode}",
            };

            var response = await client.GetAsync(QueryHelpers.AddQueryString(url, query));

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString);
                return weatherForecast;
            }
            else
            {
                //TODO: Handle error
                return null;
            }

        }
    }
}
