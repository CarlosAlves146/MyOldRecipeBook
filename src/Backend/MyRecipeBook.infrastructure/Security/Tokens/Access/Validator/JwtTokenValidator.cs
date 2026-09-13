using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Domain.Security.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Access.Validator
{
    // Validação do token
    public class JwtTokenValidator : JwtTokenHandler, IAccessTokenValidator
    {
        // chave secreta
        private readonly string _signingKey;
        public JwtTokenValidator(string signingKey)
        {
            _signingKey = signingKey;
        }

        // Validador de token
        public Guid ValidateAndGetUserIdentifier(string token)
        {
            // Alterando alguns parâmetros antes de validar.
            var validationParameter = new TokenValidationParameters
            {
                // desabilitando o parâmetro que valida para quem o token foi emitido
                ValidateAudience = false,

                // desabilitando o parâmetro que valida quem emitiu o token
                ValidateIssuer = false,

                // informando qual chave foi usada para assinar o token
                IssuerSigningKey = SecurityKey(_signingKey),

                // Remove a tolerância de tempo padrão
                ClockSkew = new TimeSpan(0),
            };

            // Responssavel por, validar, ler, interpretar JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            // método "ValidateToken" faz duas coisas ao mesmo tempo:
            // Valida o token
            // Transforma o token em um objeto utilizável pelo código ** Pode se gerar exeções aqui
            var principal = tokenHandler.ValidateToken(token, validationParameter, out _);


            // Extraindo dados, acessando a lista de claims, recuperando o primeiro claim cujo tipo seja ClaimTypes.NameIdentifier
            // extraimos o "value" que será uma string
            // var userIdentifier = principal.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value; implementação com erro!
            var userIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Convertendo o "value" para um Guid.
            return Guid.Parse(userIdentifier!);
        }
    }
}
