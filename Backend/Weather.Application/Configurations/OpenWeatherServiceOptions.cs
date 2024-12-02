namespace Weather.Application.Configurations
{
    public class OpenWeatherServiceOptions
    {
        public const string SectionName = "OpenWeatherService";

        public required string ApiKey { get; set; }
        public required string WeatherForecastURL { get; set; }
    }
}