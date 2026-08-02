using System.Text.Json.Nodes;
using GeradorDinamicoDeReceitas.Configuration;
using GeradorDinamicoDeReceitas.Models.Ollama;
using Microsoft.Extensions.Options;

namespace GeradorDinamicoDeReceitas.Services
{
    public class OllamaClient : IOllamaClient
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaSettings _settings;

        // Schema usado no campo "format" — restringe a saída do modelo a esta estrutura.
        private static readonly JsonNode RecipeJsonSchema = JsonNode.Parse("""
        {
        "type": "object",
        "properties": {
            "name": { "type": "string" },
            "description": { "type": "string" },
            "prepTimeMinutes": { "type": "integer" },
            "servings": { "type": "integer" },
            "ingredients": {
            "type": "array",
            "items": {
                "type": "object",
                "properties": {
                "item": { "type": "string" },
                "quantity": { "type": "string" }
                },
                "required": ["item", "quantity"]
            }
            },
            "instructions": {
            "type": "array",
            "items": { "type": "string" }
            },
            "approximateNutritionInfo": {
            "type": "object",
            "properties": {
                "calories": { "type": "integer" },
                "proteinG": { "type": "integer" },
                "carbsG": { "type": "integer" },
                "fatG": { "type": "integer" }
            },
            "required": ["calories", "proteinG", "carbsG", "fatG"]
            },
            "restrictionsMet": {
            "type": "array",
            "items": { "type": "string" }
            }
        },
        "required": [
            "name", "description", "prepTimeMinutes", "servings",
            "ingredients", "instructions", "approximateNutritionInfo", "restrictionsMet"
        ]
        }
        """)!;

        public OllamaClient(HttpClient httpClient, IOptions<OllamaSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> ChatAsync(string systemPrompt, string userPrompt, CancellationToken ct)
        {
            var request = new OllamaChatRequest
            {
                Model = _settings.Model,
                Stream = false,
                Format = RecipeJsonSchema,
                Options = new OllamaOptions
                {
                    Temperature = _settings.Temperature,
                    TopP = _settings.TopP,
                    RepeatPenalty = _settings.RepeatPenalty,
                    Seed = _settings.Seed
                },
                Messages = new List<OllamaMessage>
                {
                    new() { Role = "system", Content = systemPrompt },
                    new() { Role = "user", Content = userPrompt }
                }
            };

            using var response = await _httpClient.PostAsJsonAsync("/api/chat", request, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Resposta vazia do Ollama.");

            return result.Message.Content;
        }
    }
}