using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Domain.Entities;
using Shouldly;

namespace UseCases.Test.User.Profile
{
    public class GetUserProfileUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // gerando um usuário, aqui não precisaremos da password
            (var user, var _) = UserBuilder.Build();

            // criando o useCase, passando o usuário, pq dentro do build de LoggedUser o Mock vai nós devolver esse mesmo usuário
            var useCase = CreateUseCase(user);

            var result = await useCase.Execute();

            result.ShouldNotBeNull();
            result.Name.ShouldBeEquivalentTo(user.Name);
            result.Email.ShouldBeEquivalentTo(user.Email);
        }
        private static GetUserProfileUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var mapper = MapperBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);

            return new GetUserProfileUseCase(loggedUser, mapper);
        }
    }
}
