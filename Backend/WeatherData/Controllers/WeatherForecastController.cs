using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WeatherData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherService _openWeatherService;

        public WeatherForecastController(IWeatherService openWeatherService)
        {
            _openWeatherService = openWeatherService;
        }

        [Route("forecast")]
        [HttpGet]
        public async Task<ActionResult> GetForecastAsync(string city, string countryCode)
        {
            var weatherData = await _openWeatherService.GetWeatherData(city, countryCode);
            if(!string.IsNullOrEmpty(weatherData) && !string.Equals("An error has occured", weatherData))
            {
                return Ok(weatherData);
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
