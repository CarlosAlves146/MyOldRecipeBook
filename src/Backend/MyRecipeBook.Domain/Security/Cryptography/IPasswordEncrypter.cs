namespace MyRecipeBook.Domain.Security.Encrypt
{
    public interface IPasswordEncrypter
    {
        public string Encrypt(string password);
    }
}
