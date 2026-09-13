using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Access
{
    public abstract class JwtTokenHandler
    {
        // Convertendo a chave secreta em bytes
        protected static SymmetricSecurityKey SecurityKey(string signingKey)
        {
            // convertendo nossa chave secreta em bytes
            var bytes = Encoding.UTF8.GetBytes(signingKey);

            return new SymmetricSecurityKey(bytes);
        }
    }
}
