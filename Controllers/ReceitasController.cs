using GeradorDinamicoDeReceitas.Exceptions;
using GeradorDinamicoDeReceitas.Models;
using GeradorDinamicoDeReceitas.Models.Entities;
using GeradorDinamicoDeReceitas.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDinamicoDeReceitas.Controllers
{
    [ApiController]
    [Route("api/receitas")]
    public class ReceitasController : ControllerBase
    {
        private readonly IRecipeAiService _recipeAiService;
        private readonly IRecipeHistoryService _historyService;

        public ReceitasController(IRecipeAiService recipeAiService, IRecipeHistoryService historyService)
        {
            _recipeAiService = recipeAiService;
            _historyService = historyService;
        }

        [HttpPost("generate")]
        [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Generate([FromBody] RecipeRequest request, CancellationToken ct)
        {
            if (request.Ingredients is null || request.Ingredients.Count == 0)
                return BadRequest("Please provide at least one ingredient.");

            try
            {
                var recipe = await _recipeAiService.GenerateRecipeAsync(request, ct);
                await _historyService.SalvarAsync(request, recipe, ct);
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

        [HttpGet("history")]
        [ProducesResponseType(typeof(List<RecipeHistoryEntry>), StatusCodes.Status200OK)]
        public async Task<IActionResult> History([FromQuery] int page= 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var itens = await _historyService.ListAsync(page, pageSize, ct);
            return Ok(itens);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FindById(int id, CancellationToken ct)
        {
            var recipe = await _historyService.FindByIdAsync(id, ct);
            return recipe is null ? NotFound() : Ok(recipe);
        }
    }
}