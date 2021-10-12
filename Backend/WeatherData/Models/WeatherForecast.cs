using System.Collections.Generic;

namespace WeatherData.Models
{
    public class WeatherForecast
    {
        public List<Weather> weather { get; set; }
    }

    public class Weather
    {
        public string description { get; set; }
    }
}
