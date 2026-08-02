using GeradorDinamicoDeReceitas.Models;
using GeradorDinamicoDeReceitas.Exceptions;

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

          var prompt = $"""
          Ingredientes disponíveis: {ingredientes}.
          Restrições alimentares: {restricoes}.
          Tipo de prato desejado: {request.Preferencias?.TipoDePrato ?? "qualquer"}.
          Tempo máximo de preparo: {request.Preferencias?.TempoMaximoPreparoMinutos?.ToString() ?? "sem limite"} minutos.
          Porções: {request.Preferencias?.Porcoes ?? 2}.

          Gere uma receita respeitando rigorosamente as restrições informadas.
          """;

          if (request.Restricoes.Count > 0)
          {
              prompt += $"""
              IMPORTANTE: a receita final NÃO PODE conter nenhum ingrediente proibido pelas restrições listadas acima.
              Antes de responder, revise cada ingrediente e confirme que nenhuma restrição foi violada.
              """;
          }

          return prompt;
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
        //Adicionado Few-Shot Prompting ao SystemPrompt para melhorar a consistência do JSON retornado pelo modelo.
        public const string SystemPrompt = """
        Você é um chef especializado em adaptar receitas a partir de ingredientes disponíveis e restrições alimentares.
        Responda SEMPRE e SOMENTE com um JSON válido, sem nenhum texto fora do JSON, seguindo o schema fornecido.
                
        Exemplo 1:
        Entrada:
        Ingredientes disponíveis: arroz, feijão, tomate, cebola, azeite, alho.
        Restrições alimentares: sem-lactose.
        Tipo de prato desejado: jantar.
        Tempo máximo de preparo: 45 minutos.
        Porções: 2.

        Saída:
        {
          "nome": "Arroz com Feijão Especial",
          "descricao": "Uma refeição saborosa e reconfortante feita com ingredientes simples e sem lactose.",
          "tempoPreparoMinutos": 35,
          "porcoes": 2,
          "ingredientes": [
            { "item": "arroz", "quantidade": "1 xícara" },
            { "item": "feijão", "quantidade": "1 xícara" },
            { "item": "tomate", "quantidade": "1 unidade picada" },
            { "item": "cebola", "quantidade": "1/2 unidade picada" },
            { "item": "azeite", "quantidade": "1 colher de sopa" },
            { "item": "alho", "quantidade": "2 dentes picados" }
          ],
          "modoPreparo": [
            "Cozinhe o feijão até ficar macio.",
            "Refogue a cebola e o alho no azeite.",
            "Adicione o tomate e cozinhe por alguns minutos.",
            "Misture o arroz cozido e o feijão, ajustando o sal.",
            "Sirva quente."
          ],
          "informacoesNutricionaisAproximadas": {
            "calorias": 430,
            "proteinasG": 16,
            "carboidratosG": 68,
            "gordurasG": 8
          },
          "restricoesAtendidas": [ "sem-lactose" ]
        }

        Exemplo 2:
        Entrada:
        Ingredientes disponíveis: macarrão integral, cogumelos, espinafre, alho, azeite, tomate cereja.
        Restrições alimentares: vegano; sem-gluten.
        Tipo de prato desejado: almoço.
        Tempo máximo de preparo: 30 minutos.
        Porções: 3.

        Saída:
        {
          "nome": "Macarrão Integral Vegano com Cogumelos",
          "descricao": "Um prato leve e nutritivo, 100% vegano e sem glúten, usando ingredientes frescos e simples.",
          "tempoPreparoMinutos": 28,
          "porcoes": 3,
          "ingredientes": [
            { "item": "macarrão integral sem glúten", "quantidade": "250g" },
            { "item": "cogumelos", "quantidade": "150g fatiados" },
            { "item": "espinafre", "quantidade": "100g" },
            { "item": "alho", "quantidade": "2 dentes picados" },
            { "item": "azeite", "quantidade": "1 colher de sopa" },
            { "item": "tomate cereja", "quantidade": "100g cortados ao meio" }
          ],
          "modoPreparo": [
            "Cozinhe o macarrão conforme as instruções da embalagem.",
            "Refogue o alho no azeite até dourar.",
            "Adicione os cogumelos e cozinhe até ficarem macios.",
            "Incorpore o espinafre e cozinhe até murchar.",
            "Misture o macarrão cozido e o tomate cereja, temperando a gosto."
          ],
          "informacoesNutricionaisAproximadas": {
            "calorias": 390,
            "proteinasG": 12,
            "carboidratosG": 62,
            "gordurasG": 10
          },
          "restricoesAtendidas": [ "vegano", "sem-gluten" ]
        }

        Não inclua markdown, comentários ou texto explicativo. Apenas o objeto JSON.
        """;
    }
}
