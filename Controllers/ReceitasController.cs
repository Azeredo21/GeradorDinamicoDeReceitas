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
            if (request.Ingredientes is null || request.Ingredientes.Count == 0)
                return BadRequest("Informe ao menos um ingrediente.");

            try
            {
                var receita = await _recipeAiService.GerarReceitaAsync(request, ct);
                return Ok(receita);
            }
            catch (RecipeGenerationException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Não foi possível interpretar a resposta da IA.");
            }
            catch (OllamaUnavailableException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Serviço de IA indisponível no momento.");
            }
        }
    }
}