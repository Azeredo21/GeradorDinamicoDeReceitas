using System.Text.Json;
using GeradorDinamicoDeReceitas.Configuration;
using GeradorDinamicoDeReceitas.Exceptions;
using GeradorDinamicoDeReceitas.Models;
using Microsoft.Extensions.Options;

namespace GeradorDinamicoDeReceitas.Services
{
    public class RecipeAiService : IRecipeAiService
    {
        private readonly IOllamaClient _ollamaClient;
        private readonly IPromptBuilderService _promptBuilder;
        private readonly OllamaSettings _settings;
        private readonly ILogger<RecipeAiService> _logger;

        public RecipeAiService(
            IOllamaClient ollamaClient,
            IPromptBuilderService promptBuilder,
            IOptions<OllamaSettings> settings,
            ILogger<RecipeAiService> logger)
        {
            _ollamaClient = ollamaClient;
            _promptBuilder = promptBuilder;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<RecipeResponse> GerarReceitaAsync(RecipeRequest request, CancellationToken ct)
        {
            var userPrompt = _promptBuilder.BuildUserPrompt(request);
            var systemPrompt = PromptTemplates.SystemPrompt;

            Exception? lastError = null;

            for (var tentativa = 0; tentativa <= _settings.MaxRetries; tentativa++)
            {
                try
                {
                    var rawJson = await _ollamaClient.ChatAsync(systemPrompt, userPrompt, ct);
                    var receita = JsonSerializer.Deserialize<RecipeResponse>(rawJson, JsonOptions.Default)
                        ?? throw new JsonException("Deserialização retornou nulo.");

                    return receita;
                }
                catch (JsonException ex)
                {
                    lastError = ex;
                    _logger.LogWarning("Tentativa {Tentativa}: JSON inválido retornado pelo Ollama.", tentativa + 1);
                    // Na próxima tentativa, poderia reforçar o prompt pedindo correção do formato
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Falha de comunicação com o Ollama.");
                    throw new OllamaUnavailableException("Não foi possível conectar ao Ollama.", ex);
                }
            }

            throw new RecipeGenerationException("Falha ao gerar receita após múltiplas tentativas.", lastError);
        }
    }
}