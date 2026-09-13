using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.API.Token
{
    // Implementação da classe que vai acessar os headers
    public class HttpContextValue : ITokenProvider
    {
        // Responsável para nos dar acesso ao headers
        private readonly IHttpContextAccessor _contextAccessor;
        public HttpContextValue(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        // método que vai nos retorna o token do usuário
        public string Value()
        {
            var authentication = _contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

            // Removendo o trecho "Bearer" antes do return
            return authentication["Bearer ".Length..].Trim();
        }
    }
}
