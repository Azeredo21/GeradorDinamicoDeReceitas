using System.Text.Json;
using GeradorDinamicoDeReceitas.Data;
using GeradorDinamicoDeReceitas.Models;
using GeradorDinamicoDeReceitas.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeradorDinamicoDeReceitas.Services
{
    public class RecipeHistoryService : IRecipeHistoryService
    {
        private readonly RecipeDbContext _db;

        public RecipeHistoryService(RecipeDbContext db) => _db = db;

        public async Task SalvarAsync(RecipeRequest request, RecipeResponse recipe, CancellationToken ct)
        {
            var entry= new RecipeHistoryEntry
            {
                CreatedAt= DateTime.UtcNow,
                IngredientsJson = JsonSerializer.Serialize(request.Ingredients),
                RestrictionsJson = JsonSerializer.Serialize(request.Restrictions),
                RecipeJson = JsonSerializer.Serialize(recipe, JsonOptions.Default)
            };

            _db.History.Add(entry);
            await _db.SaveChangesAsync(ct);
        }

        public Task<List<RecipeHistoryEntry>> ListAsync(int page, int pageSize, CancellationToken ct) =>
            _db.History
                .OrderByDescending(h => h.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

        public async Task<RecipeResponse?> FindByIdAsync(int id, CancellationToken ct)
        {
            var entry= await _db.History.FirstOrDefaultAsync(h => h.Id == id, ct);
            return entry is null
                ? null
                : JsonSerializer.Deserialize<RecipeResponse>(entry.RecipeJson , JsonOptions.Default);
        }
    }
}