using System.Text.Json.Serialization;

namespace Weather.Domain.Models.AzureAI
{
    public class AzureAIResponse
    {
        [JsonPropertyName("choices")]
        public required Choice[] Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public required Message Message { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("content")]
        public required string Content { get; set; }
    }
}