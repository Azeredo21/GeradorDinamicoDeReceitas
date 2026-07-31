namespace GeradorDinamicoDeReceitas.Services
{
    public interface IOllamaClient
    {
        Task<string> ChatAsync(string systemPrompt, string userPrompt, CancellationToken ct);
    }
}