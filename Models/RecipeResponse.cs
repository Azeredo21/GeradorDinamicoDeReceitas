namespace GeradorDinamicoDeReceitas.Models
{
   // Models/Responses/RecipeResponse.cs
    public class RecipeResponse
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int PrepTimeMinutes { get; set; }
        public int Servings { get; set; }
        public List<RecipeIngredient> Ingredients { get; set; } = new();
        public List<string> Instructions { get; set; } = new();
        public NutritionInfo ApproximateNutritionInfo { get; set; } = default!;
        public List<string> RestrictionsMet { get; set; } = new();
    }

    public class RecipeIngredient
    {
        public string Item { get; set; } = default!;
        public string Quantity { get; set; } = default!;
    }

    public class NutritionInfo
    {
        public int Calories { get; set; }
        public int ProteinG { get; set; }
        public int CarbsG { get; set; }
        public int FatG { get; set; }
    }
}