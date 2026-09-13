using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Encrypt;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Login.Dologin
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _repository;
        private readonly IPasswordEncrypter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;

        public DoLoginUseCase(IUserReadOnlyRepository repository, IPasswordEncrypter passwordEncripter, IAccessTokenGenerator accessTokenGenerato) 
        {
          _repository = repository;   
          _passwordEncripter = passwordEncripter;
          _accessTokenGenerator = accessTokenGenerato;
        }

        public async Task<ResponsesRegisteredUserJson> Execute(RequestLoginJson request)
        {
            var encriptedPassword = _passwordEncripter.Encrypt(request.Password);

            // Antes de implementarmos esse trecho abaixo, realizamos a implementação da nossa response em caso de erro
            // a criação da class customizada InvalidLoginException e demais configurações.
            var user = await _repository.GetByEmailAndPassword(request.Email, encriptedPassword) ?? throw new InvalidLoginException();

            
            return new ResponsesRegisteredUserJson
            {
                Name = user.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier)
                }
            };
        }
    }
}
