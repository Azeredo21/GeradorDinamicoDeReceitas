using GeradorDinamicoDeReceitas.Models;
using GeradorDinamicoDeReceitas.Exceptions;

namespace GeradorDinamicoDeReceitas.Services
{
    public class PromptBuilderService : IPromptBuilderService
    {
      public string BuildUserPrompt(RecipeRequest request)
      {
          var ingredients = string.Join(", ", request.Ingredients);
          var restrictions = request.Restrictions.Count > 0
              ? string.Join("; ", request.Restrictions)
              : "none specified";

          var prompt = $"""
          Available ingredients: {ingredients}.
          Dietary restrictions: {restrictions}.
          Desired dish type: {request.Preferences?.DishType ?? "any"}.
          Maximum prep time: {request.Preferences?.MaxPrepTimeMinutes?.ToString() ?? "no limit"} minutes.
          Servings: {request.Preferences?.Servings ?? 2}.

          Generate a recipe that strictly respects the given restrictions.
          """;

          if (request.Restrictions.Count > 0)
          {
              prompt += $"""
              IMPORTANT: the final recipe MUST NOT contain any ingredient prohibited by the restrictions listed above.
              Before responding, review each ingredient and confirm that no restriction has been violated.
              """;
          }

          return prompt;
      }

      private static string TranslateRestriction(string restriction) => restriction switch
      {
          "lactose-free" => "do not use milk, cheese, butter, or any milk-derived ingredients",
          "gluten-free" => "do not use wheat, barley, rye, or any gluten-containing ingredients",
          "vegan" => "do not use any ingredients of animal origin",
          "vegetarian" => "do not use meat or seafood",
          "low-carb" => "minimize carbohydrates by avoiding sugar, refined flour, and white rice in large amounts",
          _ => restriction
      };
    }

    public static class PromptTemplates
    {
        public const string SystemPrompt = """
        You are a chef specialized in adapting recipes based on available ingredients and dietary restrictions.
        Always respond with ONLY valid JSON, with no text outside the JSON, following the provided schema.

        --- EXAMPLE 1 ---
        Input:
        Available ingredients: egg, wheat flour, milk.
        Dietary restrictions: none specified.
        Desired dish type: any.
        Maximum prep time: no limit minutes.
        Servings: 2.

        Expected output:
        {
          "name": "Simple pancakes",
          "description": "Classic pancakes, quick and easy to make.",
          "prepTimeMinutes": 15,
          "servings": 2,
          "ingredients": [
            { "item": "egg", "quantity": "2 units" },
            { "item": "wheat flour", "quantity": "1 cup" },
            { "item": "milk", "quantity": "1 cup" }
          ],
          "instructions": [
            "Whisk all the ingredients together until you get a smooth batter.",
            "Heat a non-stick pan over medium heat.",
            "Pour a ladle of batter and cook until golden on both sides."
          ],
          "approximateNutritionInfo": { "calories": 220, "proteinG": 9, "carbsG": 28, "fatG": 7 },
          "restrictionsMet": []
        }

        --- EXAMPLE 2 ---
        Input:
        Available ingredients: chicken breast, salt, oil, whole black peppercorns, brandy, beef stock, heavy cream.
        Dietary restrictions: gluten-free.
        Desired dish type: main-course.
        Maximum prep time: 30 minutes.
        Servings: 4.

        Expected output:
        {
          "name": "Chicken au poivre with creamy peppercorn sauce",
          "description": "Pan-seared chicken breasts served with a rich brandy, cracked peppercorn and cream sauce, inspired by the classic French steak au poivre.",
          "prepTimeMinutes": 28,
          "servings": 4,
          "ingredients": [
            { "item": "chicken breast, halved horizontally", "quantity": "2 large pieces (500-600g total)" },
            { "item": "salt", "quantity": "3/4 tsp" },
            { "item": "vegetable oil", "quantity": "2 tbsp" },
            { "item": "whole black peppercorns, coarsely crushed", "quantity": "2 tsp" },
            { "item": "brandy", "quantity": "80 ml" },
            { "item": "low-sodium beef stock", "quantity": "360 ml" },
            { "item": "heavy cream", "quantity": "180 ml" }
          ],
          "instructions": [
            "Season the chicken pieces with salt on both sides.",
            "Heat the oil in a large pan over high heat and sear the chicken 2-3 minutes per side until golden. Set aside.",
            "Turn off the heat, carefully pour in the brandy and scrape the browned bits from the bottom of the pan; let it bubble for about 30 seconds.",
            "Turn the heat back to high, add the beef stock and simmer until reduced by half, about 4 minutes.",
            "Stir in the cream and crushed pepper, lower to medium-high heat and simmer for 3-4 minutes until slightly thickened.",
            "Return the chicken to the pan, lower the heat and cook for another 2-3 minutes, spooning the sauce over, until heated through.",
            "Serve immediately."
          ],
          "approximateNutritionInfo": { "calories": 442, "proteinG": 35, "carbsG": 3, "fatG": 27 },
          "restrictionsMet": ["gluten-free"]
        }

        --- EXAMPLE 3 ---
        Input:
        Available ingredients: unsalted butter, sugar, lemon zest and juice, vanilla extract, eggs, milk, wheat flour.
        Dietary restrictions: vegetarian.
        Desired dish type: dessert.
        Maximum prep time: 60 minutes.
        Servings: 5.

        Expected output:
        {
          "name": "Self-saucing lemon pudding",
          "description": "A baked dessert whose batter naturally separates into two layers while baking: a light, airy sponge on top and a silky lemon custard underneath.",
          "prepTimeMinutes": 50,
          "servings": 5,
          "ingredients": [
            { "item": "unsalted butter, cubed", "quantity": "60 g" },
            { "item": "granulated sugar", "quantity": "1 cup" },
            { "item": "lemon zest", "quantity": "1 tbsp" },
            { "item": "lemon juice", "quantity": "120 ml (about 2 large lemons)" },
            { "item": "vanilla extract", "quantity": "1/2 tsp" },
            { "item": "salt", "quantity": "1 pinch" },
            { "item": "large eggs, separated", "quantity": "3 units" },
            { "item": "whole milk", "quantity": "360 ml" },
            { "item": "wheat flour", "quantity": "1/3 cup" }
          ],
          "instructions": [
            "Preheat the oven to 180°C (350°F).",
            "Melt the butter gently, either in the microwave in short bursts or over a double boiler, without letting it get too hot.",
            "Whisk the sugar, vanilla and lemon zest into the butter. Add the egg yolks and lemon juice, whisking to combine.",
            "Gradually whisk in the flour until lump-free, then whisk in the milk. The batter will be very thin.",
            "In a separate bowl, beat the egg whites to stiff peaks and gently fold them into the batter, using light scooping motions to keep the air in.",
            "Pour the batter into a baking dish set inside a larger roasting pan, and fill the outer pan with hot water halfway up the sides (water bath). Bake for about 35 minutes, until the top is golden.",
            "Let it rest for 5 minutes before serving, making sure each portion gets some of the creamy sauce from the bottom."
          ],
          "approximateNutritionInfo": { "calories": 350, "proteinG": 7, "carbsG": 52, "fatG": 14 },
          "restrictionsMet": ["vegetarian"]
        }

        Do not include markdown, comments or explanatory text. Only the JSON object.
        """;
    }
}
