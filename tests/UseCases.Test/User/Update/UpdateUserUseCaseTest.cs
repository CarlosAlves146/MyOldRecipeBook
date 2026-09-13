using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.User.Update
{
    public class UpdateUserUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange -> Prepara/Organiza
            var (user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Assert -> Verificar/Validar
            await act.ShouldNotThrowAsync();
            user.Name.ShouldBe(request.Name);
            user.Email.ShouldBe(request.Email);
        }

        [Fact]
        public async Task Error_Name_Empty()
        {
            // Arrange -> Prepara/Organiza
            var (user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;
            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Assert -> Verificar/Validar
            user.Name.ShouldNotBe(request.Name);
            user.Email.ShouldNotBe(request.Email);

            // Assert -> Verificar/Validar
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.NAME_EMPTY);
        }

        [Fact]
        public async Task Error_Email_Empty()
        {
            // Arrange -> Prepara/Organiza
            var (user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = string.Empty;
            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Assert -> Verificar/Validar
            user.Name.ShouldNotBe(request.Name);
            user.Email.ShouldNotBe(request.Email);

            // Assert -> Verificar/Validar
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.EMAIL_EMPTY);
        }

        [Fact]
        public async Task Error_Email_Invalid()
        {
            // Arrange -> Prepara/Organiza
            var (user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = "invalid.com";
            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Assert -> Verificar/Validar
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.EMAIL_INVALID);
        }

        [Fact]
        public async Task Error_Email_Already_Registered()
        {
            // Arrange -> Prepara/Organiza
            var (user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();         
            var useCase = CreateUseCase(user, request.Email);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Assert -> Verificar/Validar
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();           
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.EMAIL_ALREADY_REGISTER);
        }

        public static UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var userUpdateRepository =  new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
            var userReadOnlyRepository = new UserReadOnlyRepositoryBuilder();
            var unitOfWork = UnitOfWorkBuilder.Build();

            if (email != null)
            {
                userReadOnlyRepository.ExistActiveUserWithEmail(email);
            }
            return new UpdateUserUseCase(loggedUser, userUpdateRepository, userReadOnlyRepository.Build(), unitOfWork);
        }
    }
}
