using System.Text.Json.Nodes;

namespace GeradorDinamicoDeReceitas.Services
{
    public interface IOllamaClient
    {
        Task<string> ChatAsync(string systemPrompt, string userPrompt, JsonNode jsonSchema, CancellationToken ct);
    }
}