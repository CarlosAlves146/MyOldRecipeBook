using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Recipe.Register
{
    public class RegisterRecipeUseCaseTest
    {
        [Fact]
        public async Task Sucess()
        {
            // Arrange -> Prepara/Organiza
            (var user, _) = UserBuilder.Build();

            var request = RequestRecipeJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            var result = await useCase.Execute(request);

            // Assert -> Verificar/Validar
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNullOrWhiteSpace();
            result.Title.ShouldNotBeNullOrWhiteSpace();
            result.Title.ShouldBeEquivalentTo(request.Title);
        }
        [Fact]
        public async Task Error_Title_Empty()
        {
            // Arrange -> Prepara/Organiza
            (var user, _) = UserBuilder.Build();

            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;

            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Assert -> Verificar/Validar
            // “Execute essa função e espero que ela lance uma exceção do tipo
            //  ErrorOnValidationException”.
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

            exception.ShouldNotBeNull();
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.RECIPE_TITLE_EMPTY);
        }
        private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var mapper = MapperBuilder.Build();
            var unitOfwork = UnitOfWorkBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var writeOnlyRepository = RecipeWriteOnlyRepositoryBuilder.Build();

            return new RegisterRecipeUseCase(loggedUser, writeOnlyRepository, unitOfwork, mapper);
        }
    }
}
