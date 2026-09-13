namespace MyRecipeBook.Communication.Responses
{
    // Criamos aqui uma class de Resposta para nossa controller estamos
    // retornando o Name, caso o registro seja um sucess
    public class ResponsesRegisteredUserJson
    {
        public string Name { get; set; } = string.Empty;
        public ResponseTokensJson Tokens { get; set; } = default!;
    }
}
