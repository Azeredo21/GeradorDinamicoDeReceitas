namespace GeradorDinamicoDeReceitas.Models
{
    public class RecipeResponse
    {
        public string Nome { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public int TempoPreparoMinutos { get; set; }
        public int Porcoes { get; set; }
        public List<RecipeIngrediente> Ingredientes { get; set; } = new();
        public List<string> ModoPreparo { get; set; } = new();
        public InformacoesNutricionais InformacoesNutricionaisAproximadas { get; set; } = default!;
        public List<string> RestricoesAtendidas { get; set; } = new();
    }

    public class RecipeIngrediente
    {
        public string Item { get; set; } = default!;
        public string Quantidade { get; set; } = default!;
    }

    public class InformacoesNutricionais
    {
        public int Calorias { get; set; }
        public int ProteinasG { get; set; }
        public int CarboidratosG { get; set; }
        public int GordurasG { get; set; }
    }
}