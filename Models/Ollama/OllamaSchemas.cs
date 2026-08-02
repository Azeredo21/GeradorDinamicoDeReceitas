using System.Text.Json.Nodes;

namespace GeradorDinamicoDeReceitas.Models.Ollama
{
    public static class OllamaSchemas
    {
        public static readonly JsonNode Recipe = JsonNode.Parse("""
        {
        "type": "object",
        "properties": {
            "name": { "type": "string" },
            "description": { "type": "string" },
            "prepTimeMinutes": { "type": "integer" },
            "servings": { "type": "integer" },
            "ingredients": {
            "type": "array",
            "items": {
                "type": "object",
                "properties": {
                "item": { "type": "string" },
                "quantity": { "type": "string" }
                },
                "required": ["item", "quantity"]
            }
            },
            "instructions": { "type": "array", "items": { "type": "string" } },
            "approximateNutritionInfo": {
            "type": "object",
            "properties": {
                "calories": { "type": "integer" },
                "proteinG": { "type": "integer" },
                "carbsG": { "type": "integer" },
                "fatG": { "type": "integer" }
            },
            "required": ["calories", "proteinG", "carbsG", "fatG"]
            },
            "restrictionsMet": { "type": "array", "items": { "type": "string" } }
        },
        "required": [
            "name", "description", "prepTimeMinutes", "servings",
            "ingredients", "instructions", "approximateNutritionInfo", "restrictionsMet"
        ]
        }
        """)!;

        public static JsonNode MultipleRecipes(int quantity) => new JsonObject
        {
            ["type"] = "array",
            ["minItems"] = 1,
            ["maxItems"] = quantity,
            ["items"] = Recipe.DeepClone()
        };

        public static readonly JsonNode Substitution = JsonNode.Parse("""
        {
        "type": "object",
        "properties": {
            "suggestions": {
            "type": "array",
            "minItems": 1,
            "maxItems": 3,
            "items": {
                "type": "object",
                "properties": {
                "ingredient": { "type": "string" },
                "reason": { "type": "string" }
                },
                "required": ["ingredient", "reason"]
            }
            }
        },
        "required": ["suggestions"]
        }
        """)!;
    }
}