using System.Text.Json.Serialization;

namespace Weather.Domain.Models
{
    public class WeatherAIResponse
    {
        [JsonPropertyName("content")]
        public string Content { get; init; }

        public WeatherAIResponse(string content)
        {
            Content = content;
        }
    }
}
