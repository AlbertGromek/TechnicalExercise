namespace Weather.Domain.Models
{
    public class WeatherForecast
    {
        public required List<Weather> Weather { get; init; }
    }

    public class Weather
    {
        public required string Description { get; init; }
    }
}
