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

            for (var attempt = 0; attempt <= _settings.MaxRetries; attempt++)
            {
                try
                {
                    var rawJson = await _ollamaClient.ChatAsync(systemPrompt, userPrompt, ct);
                    var recipe = JsonSerializer.Deserialize<RecipeResponse>(rawJson, JsonOptions.Default)
                        ?? throw new JsonException("Deserialization returned null.");

                    ValidateSemantics(recipe, request);

                    return recipe;
                }
                catch (JsonException ex)
                {
                    lastError = ex;
                    _logger.LogWarning("Attempt {Attempt}: invalid JSON returned by Ollama.", attempt + 1);
                    userPrompt += $"\n\nWARNING: the previous response was malformed ({ex.Message}). " +
                                "Please fix it and respond again with only valid JSON following the schema.";
                }
                catch (RecipeValidationException ex)
                {
                    lastError = ex;
                    _logger.LogWarning("Attempt {Attempt}: {Reason}", attempt + 1, ex.Message);
                    userPrompt += $"\n\nWARNING: {ex.Message} Please generate again by correcting specifically that issue.";
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Communication failure with Ollama.");
                    throw new OllamaUnavailableException("Could not connect to Ollama.", ex);
                }
            }

            throw new RecipeGenerationException("Failed to generate recipe after multiple attempts.", lastError);
        }

        private static void ValidateSemantics(RecipeResponse recipe, RecipeRequest request)
        {
            if (recipe.Ingredients.Count == 0)
                throw new RecipeValidationException("The 'ingredients' field came back empty.");

            if (recipe.Instructions.Count == 0)
                throw new RecipeValidationException("The 'instructions' field came back empty.");

            var unconfirmedRestrictions = request.Restrictions
                .Where(r => !recipe.RestrictionsMet.Contains(r, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (unconfirmedRestrictions.Count > 0)
            {
                throw new RecipeValidationException(
                    $"The restrictions [{string.Join(", ", unconfirmedRestrictions)}] were not confirmed in the 'restrictionsMet' field.");
            }
        }
    }
}