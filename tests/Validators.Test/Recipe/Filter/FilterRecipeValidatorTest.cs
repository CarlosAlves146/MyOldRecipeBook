using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.Recipe.Filter
{
    public class FilterRecipeValidatorTest
    {
        [Fact]
        public void Sucess()
        {
            // Arrange -> Prepara/Organiza
            var validator = new FilterRecipeValidator();
            var request = RequestFilterRecipeJsonBuilder.Build();
            
            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Error_Invalid_Cooking_Time()
        {
            // Arrange -> Prepara/Organiza
            var validator = new FilterRecipeValidator();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)1000); // **

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED);
        }

        [Fact]
        public void Error_Invalid_Difficult()
        {
            // Arrange -> Prepara/Organiza
            var validator = new FilterRecipeValidator();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.Difficulties.Add((MyRecipeBook.Communication.Enums.Difficulty)1000); // **

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.DIFFICULTY_LEVEL_NOT_SUPPORTED);
        }

        [Fact]
        public void Error_Invalid_DishType()
        {
            // Arrange -> Prepara/Organiza
            var validator = new FilterRecipeValidator();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.DishTypes.Add((MyRecipeBook.Communication.Enums.DishType)1000); // **

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.DISH_TYPE_NOT_SUPPORTED);
        }
    }
}
