namespace GeradorDinamicoDeReceitas.Models.Ollama
{
    public class OllamaChatResponse
    {
        public string Model { get; set; } = default!;
        public OllamaMessage Message { get; set; } = default!;
        public bool Done { get; set; }
    }
}