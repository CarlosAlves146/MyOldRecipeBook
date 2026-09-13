using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.Recipe
{
    public class RecipeValidatorTest
    {
        [Fact]
        public void Success()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Error_Invalid_Cooking_Time()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = (MyRecipeBook.Communication.Enums.CookingTime?)1000;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();   
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED);
        }

        [Fact]
        public void Error_Invalid_Difficulty()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = (MyRecipeBook.Communication.Enums.Difficulty?)1000;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.DIFFICULTY_LEVEL_NOT_SUPPORTED);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("         ")]
        [InlineData("")]
        public void Error_Invalid_Tittle(string title)
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = title;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.RECIPE_TITLE_EMPTY);
        }

        [Fact]
        public void Success_Cooking_Time_Null()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();      
            request.CookingTime = null;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Success_Difficulty_Null()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = null;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Success_DishTypes_Empty()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.DishType.Clear();

            // OU-> request.DishType = [];

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Error_Invalid_DishTypes()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.DishType.Add((DishType)1000); // Verificar se vem do projeto de communication

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.DISH_TYPE_NOT_SUPPORTED);
        }

        [Fact]
        public void Error_Empty_Ingredients()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredient.Clear();

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.AT_LEAST_ONE_INGREDIENT);
        }

        [Fact]
        public void Error_Empty_Instructions()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.Clear();

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.AT_LEAST_ONE_INSTRUCTION);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        ")]
        public void Error_Empty_Value_Ingredients(string ingredients)
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredient.Add(ingredients);
            

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.INGREDIENT_EMPTY);
        }

        [Fact]
        public void Error_Same_Step_Instructions()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            // Atribuindo ao 1 Step o mesmo valor do ultimo Step
            request.Instructions.First().Step = request.Instructions.Last().Step;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.TWO_OR_MORE_INSTRUCTIONS_SAME_ORDER);
        }

        [Fact]
        public void Error_Negative_Step_Instructions()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Step = -1;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.NON_NEGATIVE_INSTRUCTION_STEP);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        public void Error_Empty_Value_Instructions(string instruction)
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Text = instruction;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.INSTRUCTION_EMPTY);
        }

        [Fact]
        public void Error_Instructions_Too_Long()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Text = RequestStringGenerator.Paragraphs(2001);

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            var error = result.Errors.Single();
            error.ErrorMessage.ShouldBe(ResourceMessagesException.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS);
        }
    }
}
