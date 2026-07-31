using GeradorDinamicoDeReceitas.Models;

namespace GeradorDinamicoDeReceitas.Services
{
    public interface IPromptBuilderService
    {
        string BuildUserPrompt(RecipeRequest request);
    }
}