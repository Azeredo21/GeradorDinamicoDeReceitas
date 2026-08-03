using GeradorDinamicoDeReceitas.Models;
using GeradorDinamicoDeReceitas.Models.Entities;

namespace GeradorDinamicoDeReceitas.Services
{
    public interface IRecipeHistoryService
    {
        Task SalvarAsync(RecipeRequest request, RecipeResponse recipe, CancellationToken ct);
        Task<List<RecipeHistoryEntry>> ListAsync(int page, int pageSize, CancellationToken ct);
        Task<RecipeResponse?> FindByIdAsync(int id, CancellationToken ct);
    }
}