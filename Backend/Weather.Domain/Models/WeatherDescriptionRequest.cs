namespace Weather.Domain.Models
{
    public class WeatherDescriptionRequest
    {
        public required string City { get; set; }
        public required string CountryCode { get; set; }
    }
}
