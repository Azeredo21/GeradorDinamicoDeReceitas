namespace GeradorDinamicoDeReceitas.Models.Entities
{
    public class RecipeHistoryEntry
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IngredientsJson { get; set; } = default!;
        public string RestrictionsJson { get; set; } = default!;
        public string RecipeJson { get; set; } = default!;
    }
}