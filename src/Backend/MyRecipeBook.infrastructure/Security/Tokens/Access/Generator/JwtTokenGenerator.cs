using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Access.Generator
{
    public class JwtTokenGenerator : JwtTokenHandler, IAccessTokenGenerator
    {
        // Tempo de validade token
        private readonly uint _expirationTimeMinutes;

        // Chave secreta
        private readonly string _signingKey;

        // Construtor - Injeção de depêndencia, estamos recebendo por injeção
        // a chave secreta e o tempo de expiração do token
        public JwtTokenGenerator(uint expirationTimeMinutes, string signingKey)
        {
            _expirationTimeMinutes = expirationTimeMinutes;
            _signingKey = signingKey;   
        }

        // Gerador de Token - 97
        public string Generate(Guid userIdentifier)
        {
            // identificador do usuário
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userIdentifier.ToString())
            };
           
            // Descrição do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // identifica o dono do token
                Subject = new ClaimsIdentity(claims),

                // data de expiração do token, pegando a data de agora mais a quantidade de minutos vinda por DI, appsettings.
                Expires = DateTime.UtcNow.AddMinutes(_expirationTimeMinutes),

                // Base da criação do header
                SigningCredentials = new SigningCredentials(SecurityKey(_signingKey), SecurityAlgorithms.HmacSha256Signature)
            };

            // Gerando um token a partir dessa descrição /\
            var tokenHandler = new JwtSecurityTokenHandler();

            // Criando o token passando a descrição do token
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            // retornado como "string" utilizando o WriteToken, pq é o tipo que precisamos
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
