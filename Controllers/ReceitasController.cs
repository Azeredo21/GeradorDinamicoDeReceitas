using GeradorDinamicoDeReceitas.Exceptions;
using GeradorDinamicoDeReceitas.Models;
using GeradorDinamicoDeReceitas.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDinamicoDeReceitas.Controllers
{
    [ApiController]
    [Route("api/receitas")]
    public class ReceitasController : ControllerBase
    {
        private readonly IRecipeAiService _recipeAiService;

        public ReceitasController(IRecipeAiService recipeAiService) => _recipeAiService = recipeAiService;

        [HttpPost("gerar")]
        [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Gerar([FromBody] RecipeRequest request, CancellationToken ct)
        {
            if (request.Ingredients is null || request.Ingredients.Count == 0)
                return BadRequest("Please provide at least one ingredient.");

            try
            {
                var recipe = await _recipeAiService.GerarReceitaAsync(request, ct);
                return Ok(recipe);
            }
            catch (RecipeGenerationException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, $"Error generating recipe!\n{ex.Message}");
            }
            catch (OllamaUnavailableException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "The AI service is currently unavailable.");
            }
        }
    }
}