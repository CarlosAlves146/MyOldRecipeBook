using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extension;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter
{
    public class FilterRecipeUseCase : IRecipeFilterUseCase
    {
        private readonly IRecipeReadOnlyRepository _repository;
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;
        
        public FilterRecipeUseCase(
            IMapper mapper,
            ILoggedUser user,
            IRecipeReadOnlyRepository repository
            )
        {
            _mapper = mapper;
            _loggedUser = user;
            _repository = repository;           
        }
        public async Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson request)
        {
            Validate(request);
            
            var loggedUser = await _loggedUser.User();

            // DTO (Data Transfer Object): objeto utilizado para transportar dados entre camadas de forma desacoplada.
            var filters = new Domain.Dtos.FilterRecipesDto
            {
                RecipeTitle_Ingredient = request.RecipeTitle_Ingredient,
                CookingTimes = request.CookingTimes.Distinct().Select(c => (Domain.Enums.CookingTime)c).ToList(),
                Difficulties = request.Difficulties.Distinct().Select(c => (Domain.Enums.Difficulty)c).ToList(),
                DishTypes = request.DishTypes.Distinct().Select(c => (Domain.Enums.DishType)c).ToList()
            };

            // recuperando as receitas do usuário de acordo com o filtro
            var recipes = await _repository.Filter(loggedUser, filters);


            // Retornando para a controller os resultados
            return new ResponseRecipesJson
            {
                Recipes = _mapper.Map<IList<ResponseShortsRecipeJson>>(recipes)
            };
        }

        private static void Validate(RequestFilterRecipeJson request)
        {
            var result = new FilterRecipeValidator().Validate(request);

            // 'result' é um objeto do tipo ValidationResult.
            // A propriedade "IsValid" retorna um bool indicando se a validação foi bem-sucedida.
            // Caso IsValid seja false (IsFalse()), lançamos uma exceção contendo todas as mensagens de erro.
            if (result.IsValid.IsFalse())
            {
                throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
            }
        }
    }
}
