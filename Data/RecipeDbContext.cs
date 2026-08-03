using GeradorDinamicoDeReceitas.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeradorDinamicoDeReceitas.Data
{
    public class RecipeDbContext : DbContext
    {
        public RecipeDbContext(DbContextOptions<RecipeDbContext> options) : base(options) { }

        public DbSet<RecipeHistoryEntry> History => Set<RecipeHistoryEntry>();
    }
}