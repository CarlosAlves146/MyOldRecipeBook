using MyRecipeBook.Domain.Security.Encrypt;
using System.Security.Cryptography;
using System.Text;

namespace MyRecipeBook.Infrastructure.Security.Cryptography
{
    public sealed class Sha512Encripter : IPasswordEncrypter
    {
        private readonly string _additionalKey;

        // Construtor
        public Sha512Encripter(string additionalKey)
        {
            _additionalKey = additionalKey;
        }
        public string Encrypt(string password)
        {
            // Camada temp, de segurança, adicionando uma string "ABC" para incrementar nossa Password.
            //var chaveAdcional = "ABD";
            var newPassword = $"{password}{_additionalKey}";

            // Após unir a senha recebida mais nosso reforço de senha vamos transformar a nova senha em um array de bytes
            // que representa nossa senha Ex: [0, 254, 214, 21, 100...]
            var bytes = Encoding.UTF8.GetBytes(newPassword);

            // Aqui estamos utilizando a funcaoSHA512 para gerar sempre o mesmo resultado para mesma entrada
            var hashBytes = SHA512.HashData(bytes);
            return StringBytes(hashBytes);
        }

        private static string StringBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
