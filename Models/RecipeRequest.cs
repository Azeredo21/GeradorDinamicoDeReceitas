namespace GeradorDinamicoDeReceitas.Models
{
    public class RecipeRequest
    {
        public List<string> Ingredientes { get; set; } = new();
        public List<string> Restricoes { get; set; } = new();
        public RecipePreferencias? Preferencias { get; set; }
    }

    public class RecipePreferencias
    {
        public string? TipoDePrato { get; set; }
        public int? TempoMaximoPreparoMinutos { get; set; }
        public int? Porcoes { get; set; }
    }
}