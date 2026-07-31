using GeradorDinamicoDeReceitas.Models;

namespace GeradorDinamicoDeReceitas.Services
{
    public interface IRecipeAiService
    {
        Task<RecipeResponse> GerarReceitaAsync(RecipeRequest request, CancellationToken ct);
    }
}