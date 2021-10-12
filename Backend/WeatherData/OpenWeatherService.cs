using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace WeatherData
{
    public interface IWeatherService
    {
        public Task<string> GetWeatherData(string city, string countryCode);
    }
    public class OpenWeatherService : IWeatherService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public OpenWeatherService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }
        public async Task<string> GetWeatherData(string city, string countryCode)
        {
            var apiKey = _configuration.GetValue<string>("APIKey");
            var url = _configuration.GetValue<string>("WeatherForecastURL");
            var client = _clientFactory.CreateClient();
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
                return JsonSerializer.Serialize(weatherForecast.weather.FirstOrDefault().description);
            }
            else
            {
                if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var stringstuff = await response.Content.ReadAsStringAsync();
                    var errorObject = JsonSerializer.Deserialize<WeatherErrorObject>(stringstuff, options);
                    return JsonSerializer.Serialize(errorObject.Message);
                }
                else
                {
                    return "An error has occured.";
                }
            }
        }
        public class WeatherErrorObject
        {
            public string Cod { get; set; }
            public string Message { get; set; }
        }
    }
}
