namespace GeradorDinamicoDeReceitas.Models.Ollama
{
    public class OllamaChatRequest
    {
        public string Model { get; set; } = default!;
        public List<OllamaMessage> Messages { get; set; } = new();
        public string Format { get; set; } = "json"; // força saída JSON válida
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
        public double Temperature { get; set; } = 0.4;
    }
}