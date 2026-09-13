using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Attributes;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.API.Controllers
{
    public class UsersController : MyRecipeBookBaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponsesRegisteredUserJson), StatusCodes.Status201Created)]
        // Criamos aqui /\ um atributo informativo mas muito útil, add ele acima do método para informar
        // qual o tipo de dado que a API vai retornar no caso, "ResponsesRegisteredUserJson",
        // e qual o código HTTP que será retornado no caso "StatusCodes.Status201Created"

        // Aqui, em nosso endpoint vamos recuperar nosso IRegisterUserUseCase.
        // para isso vamos []especificar da onde está vindo.
        public async Task<IActionResult> Register(
            [FromServices] IRegisterUserUseCase useCase,
            [FromBody] RequestRegisterUserJson request)
        {
            // Criamos aqui uma instância de RegisterUserUseCase... para utilizar os métodos que lá estão.           
            // var useCase = new RegisterUserUseCase(); *linha comentada pq agora estamos recebendo por injeção de dependência

            var result = await useCase.Execute(request);

            // Lembrando que o Created recebe nenhum ou dois parametros, como só temos UM parâmetro para retornar
            // adicionamos uma string.Empty, para não dar erro.
            return Created(string.Empty, 
                new ResponsesRegisteredUserJson { Name = result.Name, Tokens = result.Tokens });
        }

        // endPoint que recupera informações do usuário de acordo com seu token
        [HttpGet]
        [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
        [AuthenticatedUser]
        public async Task<IActionResult> GetUserProfile([FromServices] IGetUserProfileUseCase useCase)
        {
            var result = await useCase.Execute();

            return Ok(result);
        }

        // Update name/email do usuário
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AuthenticatedUser]
        public async Task<IActionResult> Update([FromServices] IUpdateUserUseCase useCase, [FromBody] RequestUpdateUserJson request)
        {
            await useCase.Execute(request);

            return NoContent();
        }

        // Update alterando a senha
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AuthenticatedUser]
        public async Task<IActionResult> ChangePassword([FromServices] IChangePasswordUseCase useCase, [FromBody] RequestChangePasswordJson request)
        {
            await useCase.Execute(request);

            return NoContent();
        }
    }
}
