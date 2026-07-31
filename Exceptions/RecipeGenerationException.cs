namespace GeradorDinamicoDeReceitas.Exceptions
{
    public class RecipeGenerationException : Exception
    {
        public RecipeGenerationException(string message) : base(message) { }

        public RecipeGenerationException(string message, Exception? innerException)
            : base(message, innerException!) { }
    }
}