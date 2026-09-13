using CommonTestUtilities.Cryptography;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Google.Protobuf.WellKnownTypes;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.User.ChangePassword
{
    public class ChangePasswordUseCaseTest
    {
        [Fact]
        public async Task Sucess()
        {
            // Arrange -> Prepara/Organiza
            var (user, Password) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = Password;
                            
            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);
        
            // Acert
            await act.ShouldNotThrowAsync();

            var passwordEncripter = PasswordEncripterBuilder.Build();
            // Comparando as cript. após a alteração
            user.Password.ShouldBe(passwordEncripter.Encrypt(request.NewPassword));
        }
        [Fact]
        public async Task Error_NewPassword_Empty()
        {
            // Arrange -> Prepara/Organiza
            var (user, Password) = UserBuilder.Build();
            var request = new RequestChangePasswordJson
            {
                CurrentPassword = Password,
                NewPassword = string.Empty                
            };

            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Acert
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.PASSWORD_EMPTY);            
        }
        [Fact]
        public async Task Error_NewPassword_Below_Digits()
        {
            // Arrange -> Prepara/Organiza
            var (user, Password) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build(2);
            request.CurrentPassword = Password;
            
            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Acert
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.PASSWORD_NUMBER_DIGITS_BELOW);
        }
        [Fact]
        public async Task Error_CurrentPassword_Different()
        {
            // Arrange -> Prepara/Organiza
            var (user, Password) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
           
            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);

            // Acert
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.PASSWORD_DIFFERENT_CURRENT_PASSWORD);
        }
        private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var loggerUser = LoggedUserBuilder.Build(user);
            var userUpdateOnlyRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var passwordEncrypter = PasswordEncripterBuilder.Build();

            return new ChangePasswordUseCase(loggerUser, userUpdateOnlyRepository, unitOfWork, passwordEncrypter);
        }
    }
}
