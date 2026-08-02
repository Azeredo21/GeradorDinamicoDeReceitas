using GeradorDinamicoDeReceitas.Models;

namespace GeradorDinamicoDeReceitas.Services
{
    public interface IRecipeAiService
    {
        Task<RecipeResponse> GenerateRecipeAsync(RecipeRequest request, CancellationToken ct);
    }
}