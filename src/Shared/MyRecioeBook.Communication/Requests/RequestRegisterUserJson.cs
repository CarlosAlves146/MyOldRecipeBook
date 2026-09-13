namespace MyRecipeBook.Communication.Requests
{
    public class RequestRegisterUserJson
    {
        public string Name { get; set; } = string.Empty; //para questão de estudo, é utilizado o string.Empty, p/ que se o usuario ñ enviar o valor, ele passa a ser vazio e ñ nulo.
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
