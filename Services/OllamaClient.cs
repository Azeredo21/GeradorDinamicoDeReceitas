using GeradorDinamicoDeReceitas.Configuration;
using GeradorDinamicoDeReceitas.Models.Ollama;
using Microsoft.Extensions.Options;

namespace GeradorDinamicoDeReceitas.Services
{
    public class OllamaClient : IOllamaClient
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaSettings _settings;

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
                Format = "json",
                Options = new OllamaOptions { Temperature = _settings.Temperature },
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