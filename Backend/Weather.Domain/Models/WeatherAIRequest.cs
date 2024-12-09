using System.ComponentModel.DataAnnotations;

namespace Weather.Domain.Models
{
    public class WeatherAIRequest
    {
        [Required]
        public required string Description { get; init; }
        [Required]
        public required string City { get; init; }
        [Required]
        public required string Country { get; init; }
    }
}
