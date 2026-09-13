using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.User.Register
{
    public class RegisterUserUseCaseTest
    {
        [Fact]
        public async Task Sucess()
        {
            // Arrange -> Prepara/Organiza
            var request = RequestRegisterUserJsonBuilder.Build();

            var useCase = CreateUseCase();

            // Act -> Agir/Executar
            var result = await useCase.Execute(request);

            // Assert -> Verificar/Validar
            result.ShouldNotBeNull();
            result.Tokens.ShouldNotBeNull();
            result.Name.ShouldBeEquivalentTo(request.Name);
            result.Tokens.AccessToken.ShouldNotBeNullOrEmpty();
        }
        [Fact]
        public async Task Erro_Email_Already_Registered()
        {
            // Arrange -> Prepara/Organiza
            var request = RequestRegisterUserJsonBuilder.Build();
            var useCase = CreateUseCase(request.Email);

            // ACT → Aqui executamos a ação que queremos testar.
            // Func<Task> significa: “um método que será executado depois para testar exceções”.
            Func<Task> act = async () => await useCase.Execute(request);

            // “Execute essa função e espero que ela lance uma exceção do tipo
            //  ErrorOnValidationException”.
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // ASSERT → Agora validamos se o erro realmente é o que esperávamos.
            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.EMAIL_ALREADY_REGISTER);
        }

        [Fact]
        public async Task Erro_Name_Empty()
        {
            // Arrange -> Prepara/Organiza
            var request = RequestRegisterUserJsonBuilder.Build();
            var useCase = CreateUseCase();
            request.Name = string.Empty;

            Func<Task> act = async () => await useCase.Execute(request);
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

            exception.ErrorMessages.ShouldHaveSingleItem();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.NAME_EMPTY);
        }

        // Builder for UseCase
        private static RegisterUserUseCase CreateUseCase(string? email = null)
        {
            var mapper = MapperBuilder.Build();
            var passwordEncripter = PasswordEncripterBuilder.Build();
            var unitOfwork = UnitOfWorkBuilder.Build();
            var readOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            var writeOnlyRepository = UserWriteOnlyRepositoryBuilder.Build();
            var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();

            if (string.IsNullOrEmpty(email) == false)
            {
                readOnlyRepositoryBuilder.ExistActiveUserWithEmail(email);
            }

            return new RegisterUserUseCase(writeOnlyRepository, readOnlyRepositoryBuilder.Build(), unitOfwork, mapper, passwordEncripter, accessTokenGenerator);
        }
    }
}
