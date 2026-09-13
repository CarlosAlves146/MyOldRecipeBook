using Moq;
using MyRecipeBook.Domain.Repositories;

namespace CommonTestUtilities.Repositories
{
    public class UnitOfWorkBuilder
    {
        public static IUnitOfWork Build()
        {
            // Aqui criamos um "mock" (uma imitação) da interface IUnitOfWork.
            // Pense assim: "Mock, finja ser um IUnitOfWork, mas sem fazer nada de verdade."
            var mock = new Mock<IUnitOfWork>();

            // Aqui devolvemos o "objeto fake" gerado pelo mock.
            // Esse objeto se comporta como um IUnitOfWork,
            // mas não vai acessar banco, nem fazer commit real.
            return mock.Object;
        }
    }
}
