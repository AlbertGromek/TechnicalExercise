using System.Text.Json.Serialization;

namespace Weather.Domain.Models
{
    public class WeatherAIResponse(string content)
    {
        [JsonPropertyName("content")]
        public string Content { get; init; } = content;
    }
}
