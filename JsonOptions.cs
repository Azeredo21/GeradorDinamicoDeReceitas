using System.Text.Json;

namespace GeradorDinamicoDeReceitas
{
    public static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
}