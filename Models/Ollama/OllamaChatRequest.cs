using System.Text.Json.Serialization;

namespace GeradorDinamicoDeReceitas.Models.Ollama
{
    public class OllamaChatRequest
    {
        public string Model { get; set; } = default!;
        public List<OllamaMessage> Messages { get; set; } = new();
        // JSON Schema completo (JsonNode), não apenas a string "json".
        // Restringe a decodificação do modelo ao schema informado (structured outputs).
        public object Format { get; set; } = default!;  
        public bool Stream { get; set; } = false;
        public OllamaOptions? Options { get; set; }
    }

    public class OllamaMessage
    {
        public string Role { get; set; } = default!; // "system" | "user" | "assistant"
        public string Content { get; set; } = default!;
    }

    public class OllamaOptions
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.2;

        [JsonPropertyName("top_p")]
        public double TopP { get; set; } = 0.9;

        [JsonPropertyName("repeat_penalty")]
        public double RepeatPenalty { get; set; } = 1.1;

        [JsonPropertyName("seed")]
        public int? Seed { get; set; }
    }
}