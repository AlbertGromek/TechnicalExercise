namespace Weather.Domain.Models.OpenWeatherService
{
    public class WeatherErrorObject
    {
        public required string Cod { get; init; }
        public required string Message { get; init; }
    }
}
