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
            var systemPrompt = PromptTemplates.SystemPrompt;
            var userPrompt = _promptBuilder.BuildUserPrompt(request);

            Exception? lastError = null;

            for (var tentativa = 0; tentativa <= _settings.MaxRetries; tentativa++)
            {
                try
                {
                    var rawJson = await _ollamaClient.ChatAsync(systemPrompt, userPrompt, ct);
                    var receita = JsonSerializer.Deserialize<RecipeResponse>(rawJson, JsonOptions.Default)
                        ?? throw new JsonException("Deserialização retornou nulo.");

                    ValidarSemantica(receita, request);

                    return receita;
                }
                catch (JsonException ex)
                {
                    lastError = ex;
                    _logger.LogWarning("Tentativa {Tentativa}: JSON inválido retornado pelo Ollama.", tentativa + 1);
                    userPrompt += $"\n\nATENÇÃO: a resposta anterior estava malformada ({ex.Message}). " +
                                "Corrija e responda novamente apenas com o JSON válido, seguindo o schema.";
                }
                catch (RecipeValidationException ex)
                {
                    lastError = ex;
                    _logger.LogWarning("Tentativa {Tentativa}: {Motivo}", tentativa + 1, ex.Message);
                    userPrompt += $"\n\nATENÇÃO: {ex.Message} Gere novamente corrigindo especificamente esse ponto.";
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Falha de comunicação com o Ollama.");
                    throw new OllamaUnavailableException("Não foi possível conectar ao Ollama.", ex);
                }
            }

            throw new RecipeGenerationException("Falha ao gerar receita após múltiplas tentativas.", lastError);
        }

        private static void ValidarSemantica(RecipeResponse receita, RecipeRequest request)
        {
            if (receita.Ingredientes.Count == 0)
                throw new RecipeValidationException("O campo 'ingredientes' veio vazio.");

            if (receita.ModoPreparo.Count == 0)
                throw new RecipeValidationException("O campo 'modoPreparo' veio vazio.");

            var restricoesNaoConfirmadas = request.Restricoes
                .Where(r => !receita.RestricoesAtendidas.Contains(r, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (restricoesNaoConfirmadas.Count > 0)
            {
                throw new RecipeValidationException(
                    $"As restrições [{string.Join(", ", restricoesNaoConfirmadas)}] não foram confirmadas no campo 'restricoesAtendidas'.");
            }
        }
    }
}