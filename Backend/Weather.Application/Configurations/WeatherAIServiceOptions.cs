namespace Weather.Application.Configurations
{
    public class WeatherAIServiceOptions
    {
        public const string SectionName = "WeatherAIService";

        public required string ApiKey { get; set; }
        public required string Endpoint { get; set; }
    }
}