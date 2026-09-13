using System.Runtime.CompilerServices;

namespace MyRecipeBook.Exceptions.ExceptionBase
{
    // Criamos aqui nosso tipo de Exception, lembrando de herdar de nossa MyRecipeBookException para que ela tbm seja reconhecida
    // como uma Exception.
    // Aqui vamos receber uma lista de erros da nossa validação e vamos armazenar ele em ErrorMessages
    // e agora nosso sistema automaticamente vai executar nosso "FitroGlobal" que foi inserido lá em Program.cs
    public class ErrorOnValidationException : MyRecipeBookException
    {
        public IList<string> ErrorMessages { get; set; }

        // Construtor.
        public ErrorOnValidationException(IList<string> errorMessages) : base(string.Empty)
        {
            ErrorMessages = errorMessages;
        }
    }
}
