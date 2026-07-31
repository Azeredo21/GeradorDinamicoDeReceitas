using GeradorDinamicoDeReceitas.Models;

namespace GeradorDinamicoDeReceitas.Services
{
    public class PromptBuilderService : IPromptBuilderService
    {
        public string BuildUserPrompt(RecipeRequest request)
        {
            var ingredientes = string.Join(", ", request.Ingredientes);
            var restricoes = request.Restricoes.Count > 0
                ? string.Join("; ", request.Restricoes.Select(TraduzirRestricao))
                : "nenhuma restrição específica";

            return $"""
            Ingredientes disponíveis: {ingredientes}.
            Restrições alimentares: {restricoes}.
            Tipo de prato desejado: {request.Preferencias?.TipoDePrato ?? "qualquer"}.
            Tempo máximo de preparo: {request.Preferencias?.TempoMaximoPreparoMinutos?.ToString() ?? "sem limite"} minutos.
            Porções: {request.Preferencias?.Porcoes ?? 2}.

            Gere uma receita respeitando rigorosamente as restrições informadas.
            """;
        }

        private static string TraduzirRestricao(string restricao) => restricao switch
        {
            "sem-lactose" => "não utilizar leite, queijo, manteiga ou qualquer derivado de leite",
            "sem-gluten" => "não utilizar trigo, cevada, centeio ou derivados contendo glúten",
            "vegano" => "não utilizar nenhum ingrediente de origem animal",
            "vegetariano" => "não utilizar carnes ou frutos do mar",
            "low-carb" => "minimizar carboidratos, evitando açúcar, farinha refinada e arroz branco em grande quantidade",
            _ => restricao
        };
    }

    public static class PromptTemplates
    {
        public const string SystemPrompt = """
        Você é um chef especializado em adaptar receitas a partir de ingredientes disponíveis e restrições alimentares.
        Responda SEMPRE e SOMENTE com um JSON válido, sem nenhum texto fora do JSON, seguindo exatamente este schema:

        {
        "nome": string,
        "descricao": string,
        "tempoPreparoMinutos": number,
        "porcoes": number,
        "ingredientes": [ { "item": string, "quantidade": string } ],
        "modoPreparo": [ string ],
        "informacoesNutricionaisAproximadas": {
            "calorias": number, "proteinasG": number, "carboidratosG": number, "gordurasG": number
        },
        "restricoesAtendidas": [ string ]
        }

        Não inclua markdown, comentários ou texto explicativo. Apenas o objeto JSON.
        """;
    }
}