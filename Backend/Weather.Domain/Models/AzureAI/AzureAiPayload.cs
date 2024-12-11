using System.Text.Json.Serialization;

namespace Weather.Domain.Models.AzureAI
{
    public class AzureAIPayload
    {
        [JsonPropertyName("messages")]
        public required AzureAIMessage[] Messages { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("top_p")]
        public double TopP { get; set; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }
    }

    public class AzureAIMessage
    {
        [JsonPropertyName("role")]
        public required string Role { get; set; }

        [JsonPropertyName("content")]
        public required AzureAIContent[] Content { get; set; }
    }

    public class AzureAIContent
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("text")]
        public required string Text { get; set; }
    }
}