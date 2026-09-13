using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories
{
    public class UserWriteOnlyRepositoryBuilder
    {
        public static IUserWriteOnlyRepository Build()
        {
            // Aqui criamos um "mock" (uma imitação) da interface IUserWriteOnlyRepository.
            // Pense assim: "Mock, finja ser um IUserWriteOnlyRepository, mas sem fazer nada de verdade."
            var mock = new Mock<IUserWriteOnlyRepository>();
           
            // Aqui devolvemos o "objeto fake" gerado pelo mock.
            // Esse objeto se comporta como um IUserWriteOnlyRepository,
            // mas não vai acessar banco, nem faz commit real.
            return mock.Object;
        }
    }
}
