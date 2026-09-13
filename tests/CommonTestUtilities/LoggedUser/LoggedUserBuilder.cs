using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.LoggedUser;

namespace CommonTestUtilities.LoggedUser
{
    public class LoggedUserBuilder
    {
        public static ILoggedUser Build(User user)
        {
            var mock = new Mock<ILoggedUser>();

            // Quando chamarem o método User() da interface ILoggedUser...Retorne esse user que eu recebi no parâmetro.
            mock.Setup(x => x.User()).ReturnsAsync(user);
            return mock.Object;
        }
    }
}
