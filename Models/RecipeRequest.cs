namespace GeradorDinamicoDeReceitas.Models
{
    public class RecipeRequest
    {
        public List<string> Ingredients { get; set; } = new();
        public List<string> Restrictions { get; set; } = new();
        public RecipePreferencias? Preferences { get; set; }
    }

    public class RecipePreferencias
    {
        public string? DishType { get; set; }
        public int? MaxPrepTimeMinutes { get; set; }
        public int? Servings { get; set; }
    }
}