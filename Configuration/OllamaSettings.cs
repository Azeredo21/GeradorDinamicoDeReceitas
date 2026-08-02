namespace GeradorDinamicoDeReceitas.Configuration
{
    public class OllamaSettings
    {
        public string BaseUrl { get; set; } = "http://localhost:11434";
        public string Model { get; set; } = "llama3.1:8b";
        public int TimeoutSeconds { get; set; } = 60;
        public double Temperature { get; set; } = 0.2;
        public double TopP { get; set; } = 0.9;
        public double RepeatPenalty { get; set; } = 1.1;
        public int? Seed { get; set; }
        public int MaxRetries { get; set; } = 2;
    }
}