using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Recipe.Filter
{
    public class FilterRecipeUseCaseTest
    {

        [Fact]
        public async Task Success()
        {
            // Arrange -> Prepara/Organiza
            (var user, _) = UserBuilder.Build();
            var request = RequestFilterRecipeJsonBuilder.Build();

            var recipes = RecipeBuilder.Collection(user);

            var useCase = CreateUseCase(user, recipes);

            // Act -> Agir/Executar
            var result = await useCase.Execute(request);

            // Assert -> Verificar/Validar
            result.ShouldNotBeNull();
            result.Recipes.ShouldNotBeNull().ShouldNotBeEmpty();
            result.Recipes.Count.ShouldBeEquivalentTo(recipes.Count);
            result.Recipes.First().Title.ShouldBe(recipes.First().Title.ToUpper()); 
            // I had to include a method "ToUpper", Because of return for user in UseCase on property "Tittle"
            // have a method for change tittle in upper case
        }

        [Fact]
        public async Task Error_CookingTime_Invalid()
        {
            // Arrange -> Prepara/Organiza
            (var user, _) = UserBuilder.Build();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)1000);

            var recipes = RecipeBuilder.Collection(user);

            var useCase = CreateUseCase(user, recipes);

            // Act -> Agir/Executar
            Func<Task> act = async () => { await useCase.Execute(request); };


            // Assert -> Verificar/Validar
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
            exception.ErrorMessages.ShouldNotBeNull();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED);  
        }

        [Fact]
        public async Task Error_DishType_Invalid()
        {
            // Arrange -> Prepara/Organiza
            (var user, _) = UserBuilder.Build();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.DishTypes.Add((MyRecipeBook.Communication.Enums.DishType)1000);

            var recipes = RecipeBuilder.Collection(user);

            var useCase = CreateUseCase(user, recipes);

            // Act -> Agir/Executar
            Func<Task> act = async () => await useCase.Execute(request);


            // Assert -> Verificar/Validar
            var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
            exception.ErrorMessages.ShouldNotBeNull();
            exception.ErrorMessages.ShouldContain(ResourceMessagesException.DISH_TYPE_NOT_SUPPORTED);
        }

        private static FilterRecipeUseCase CreateUseCase(
            MyRecipeBook.Domain.Entities.User user, 
            IList<MyRecipeBook.Domain.Entities.Recipe> recipes)
        {
            var mapper = MapperBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var repository = new RecipeReadOnlyRepositoryBuilder().ConfigFilter(user, recipes).Build();           

            return new FilterRecipeUseCase(mapper, loggedUser, repository);
        }
    }
}
