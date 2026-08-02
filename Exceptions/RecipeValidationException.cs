namespace GeradorDinamicoDeReceitas.Exceptions
{
    public class RecipeValidationException : Exception
    {
        public RecipeValidationException(string message) : base(message) { }
    }
}