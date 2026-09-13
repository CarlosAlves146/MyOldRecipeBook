using MyRecipeBook.Domain.Security.Encrypt;
using MyRecipeBook.Infrastructure.Security.Cryptography;

namespace CommonTestUtilities.Cryptography
{
    public class PasswordEncripterBuilder
    {
        public static IPasswordEncrypter Build()
        {
            string additionalKey = "ABC123";
            return new Sha512Encripter(additionalKey);
        }
    }
}
