namespace MyRecipeBook.Exceptions.ExceptionBase
{
    // Lembrando que aqui precisamos herdar de SystemException, para transformar
    // nossa class em um tipo de Exception
    public class MyRecipeBookException : SystemException
    {       
        public MyRecipeBookException(string message) : base(message) { }
    }
}
