using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Access.Generator;

namespace CommonTestUtilities.Tokens
{
    // Para essa implementação precisar add a dependência ao projeto de infraestrutura, para poder ter acesso a JwtTokenGenerator
    public class JwtTokenGeneratorBuilder
    {
        //public static IAccessTokenGenerator Build() => new JwtTokenGenerator(expirationTimeMinutes: 5, signingKey: "tttttttttttttttttttttttttttttttt");
        public static IAccessTokenGenerator Build()
        {
            return new JwtTokenGenerator(expirationTimeMinutes: 5, signingKey: "tttttttttttttttttttttttttttttttt");

        }
    }
}
