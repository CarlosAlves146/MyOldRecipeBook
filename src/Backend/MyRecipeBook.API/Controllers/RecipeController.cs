using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Attributes;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers
{
    [AuthenticatedUser]
    public class RecipesController : MyRecipeBookBaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponsesRegisteredRecipeJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
            [FromServices] IRegisterRecipeUseCase useCase, 
            [FromBody] RequestRecipeJson request)
        {
            var result = await useCase.Execute(request);
                       
            return Created(string.Empty,
               new ResponsesRegisteredRecipeJson { Id = result.Id, Title = result.Title });
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(ResponseRecipesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Filter(
            [FromServices] IRecipeFilterUseCase useCase,
            [FromBody] RequestFilterRecipeJson request)
        {
            var result = await useCase.Execute(request);

            // Verificamos se foi encontrado receitas no banco de dados
            if(result.Recipes.Any())
            {
                return Ok(result);
            }

            return NoContent();       
        }
    }
}
