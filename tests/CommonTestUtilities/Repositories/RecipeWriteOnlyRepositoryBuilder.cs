using Moq;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtilities.Repositories
{
    public class RecipeWriteOnlyRepositoryBuilder
    {
        public static IRecipeWriteOnlyRepository Build()
        {
            // Aqui criamos um "mock" (uma imitação) da interface IRecipeWriteOnlyRepository.
            // Pense assim: "Mock, finja ser um IRecipeWriteOnlyRepository, mas sem fazer nada de verdade."
            var mock = new Mock<IRecipeWriteOnlyRepository>();

            // Aqui devolvemos o "objeto fake" gerado pelo mock.
            // Esse objeto se comporta como um IRecipeWriteOnlyRepository,
            // mas não vai acessar banco, nem faz commit real.
            return mock.Object;
        }
    }
}
