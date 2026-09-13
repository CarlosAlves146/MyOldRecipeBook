using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extension;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.API.Filters
{
    public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
    {
        // responsável por validar/autenticar o token
        private readonly IAccessTokenValidator _accessTokenValidator;

        // (DI) responsavel por buscar o userIdentifier no banco
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;

        // Construtor
        public AuthenticatedUserFilter(
            IAccessTokenValidator accessTokenValidator,
            IUserReadOnlyRepository userReadOnlyRepository)
        {
            _accessTokenValidator = accessTokenValidator;
            _userReadOnlyRepository = userReadOnlyRepository;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var token = TokenOnRequest(context);

                // Método da (DI) que valída o token **Durante essa validação pode ser gerado algumas exceções
                var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);

                // Método da (DI) que verifica a existência desse identifier no banco de dados retornando um "bool"
                var exist = await _userReadOnlyRepository.ExistActiveUserWithIdentifier(userIdentifier);

                // Lembrando que esse método "IsFalse" foi criado para seguir os critérios do Sonar
                if (exist.IsFalse())
                {
                    // Caso não seja encontrado esse "userIdentifier" no banco de dados, vamos lançar essa exception
                    throw new MyRecipeBookException(ResourceMessagesException.USER_WITHOUT_PREMISSION_ACCESS_RESOURCE);
                }
            }
            // 102-18:25
            catch (SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("TokenIsExpired")
                {
                    TokenIsExpired = true,
                });
            }
            catch (MyRecipeBookException ex)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message));
            }
            catch
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.USER_WITHOUT_PREMISSION_ACCESS_RESOURCE));
            }
        }
        private static string TokenOnRequest(AuthorizationFilterContext context)
        {
            // recuperando o token do Header - como string
            var authentication = context.HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authentication))
            {
                // Caso no header não tenha um token, lançaremos um erro
                throw new MyRecipeBookException(ResourceMessagesException.NO_TOKEN);
            }
            // Removendo o trecho "Bearer" antes do return
            return authentication["Bearer ".Length..].Trim();
        }
    }
}
