using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.Security.Tokens.Access;
using System.IdentityModel.Tokens.Jwt;

namespace MyRecipeBook.Infrastructure.Services.LoggedUser
{
    // Classe responsável por buscar no banco um usuário correspondente ao token
    public class LoggedUser : ILoggedUser
    {
        private readonly MyRecipeBookDbContext _dbContext;

        // (DI) responsável por recuperar do headers o token
        private readonly ITokenProvider _tokenProvider;
        public LoggedUser(MyRecipeBookDbContext dbContext, ITokenProvider tokenProvider)
        {
            _dbContext = dbContext;
            _tokenProvider = tokenProvider;
        }
        public async Task<User> User()
        {           
            // método que recupera o token da Request.headers, Teste
            var token = _tokenProvider.Value();

            // Criando o leitor do token
            var tokenHandler = new JwtSecurityTokenHandler();

            // lendo
            var JwtSecurityToken = tokenHandler.ReadJwtToken(token);

            // pegando o valor da Claim "sub"
            var identifier = JwtSecurityToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
           
            var userIdentifier = Guid.Parse(identifier);

            // Buscando o Usuário no Banco
            return await _dbContext
                .Users
                .AsNoTracking()
                .FirstAsync(user => user.Active && user.UserIdentifier == userIdentifier);
        }
    }
}
