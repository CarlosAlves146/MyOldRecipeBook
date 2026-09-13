using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter
{
    public interface IRecipeFilterUseCase
    {
        public Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson filter);
    }
}
