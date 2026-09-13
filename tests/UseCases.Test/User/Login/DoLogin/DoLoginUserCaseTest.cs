using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.Login.Dologin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.User.Login.DoLogin
{
    public class DoLoginUserCaseTest
    {
        [Fact]
        public async Task Sucess()
        {
            // Arrange -> Prepara/Organiza
            (var user, var password) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            // Act -> Agir/Executar
            var result = await useCase.Execute(new RequestLoginJson
            {
                Email = user.Email,
                Password = password,
            });

            // Assert -> Verificar/Validar
            result.ShouldNotBeNull();
            result.Tokens.ShouldNotBeNull();
            result.Name.ShouldNotBeNullOrWhiteSpace();
            result.Name.ShouldBe(user.Name);
            result.Tokens.AccessToken.ShouldNotBeNullOrEmpty();     
        }
        [Fact]
        public async Task Error_Invalid_User()
        {
            // Arrange -> Prepara/Organiza
            var request = RequestLoginUserJsonBuilder.Build();

            var useCase = CreateUseCase();

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request); 

            // Assert -> Verificar/Validar
            var exception = await act.ShouldThrowAsync<InvalidLoginException>();

            exception.Message.ShouldContain(ResourceMessagesException.EMAIL_OR_PASSWORD_INVALID);
        }

        private static DoLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
        {
            var readOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            var passwordEncripter = PasswordEncripterBuilder.Build();
            var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
            
            if (user != null) 
            {
                readOnlyRepositoryBuilder.GetByEmailAndPassword(user);
            }
            return new DoLoginUseCase(readOnlyRepositoryBuilder.Build(), passwordEncripter, accessTokenGenerator);
        }
    }   
}
