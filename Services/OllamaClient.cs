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
            "nome": { "type": "string" },
            "descricao": { "type": "string" },
            "tempoPreparoMinutos": { "type": "integer" },
            "porcoes": { "type": "integer" },
            "ingredientes": {
            "type": "array",
            "items": {
                "type": "object",
                "properties": {
                "item": { "type": "string" },
                "quantidade": { "type": "string" }
                },
                "required": ["item", "quantidade"]
            }
            },
            "modoPreparo": {
            "type": "array",
            "items": { "type": "string" }
            },
            "informacoesNutricionaisAproximadas": {
            "type": "object",
            "properties": {
                "calorias": { "type": "integer" },
                "proteinasG": { "type": "integer" },
                "carboidratosG": { "type": "integer" },
                "gordurasG": { "type": "integer" }
            },
            "required": ["calorias", "proteinasG", "carboidratosG", "gordurasG"]
            },
            "restricoesAtendidas": {
            "type": "array",
            "items": { "type": "string" }
            }
        },
        "required": [
            "nome", "descricao", "tempoPreparoMinutos", "porcoes",
            "ingredientes", "modoPreparo", "informacoesNutricionaisAproximadas", "restricoesAtendidas"
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