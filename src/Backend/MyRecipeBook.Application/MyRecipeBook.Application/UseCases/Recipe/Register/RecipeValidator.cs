using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using System.Runtime.Intrinsics.X86;

namespace MyRecipeBook.Application.UseCases.Recipe.Register
{
    public class RecipeValidator : AbstractValidator<RequestRecipeJson>
    {
        public RecipeValidator()
        {
            RuleFor(recipe => recipe.Title).NotEmpty().WithMessage(ResourceMessagesException.RECIPE_TITLE_EMPTY);
            RuleFor(recipe => recipe.CookingTime).IsInEnum().WithMessage(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED);
            RuleFor(recipe => recipe.Difficulty).IsInEnum().WithMessage(ResourceMessagesException.DIFFICULTY_LEVEL_NOT_SUPPORTED);
            RuleFor(recipe => recipe.Ingredient.Count).GreaterThan(0).WithMessage(ResourceMessagesException.AT_LEAST_ONE_INGREDIENT);
            RuleFor(recipe => recipe.Instructions.Count).GreaterThan(0).WithMessage(ResourceMessagesException.AT_LEAST_ONE_INSTRUCTION);

            RuleForEach(recipe => recipe.Ingredient).NotEmpty().WithMessage(ResourceMessagesException.INGREDIENT_EMPTY);
            RuleForEach(recipe => recipe.DishType).IsInEnum().WithMessage(ResourceMessagesException.DISH_TYPE_NOT_SUPPORTED);
            RuleForEach(recipe => recipe.Instructions).ChildRules(instructionsRule =>
            {
                instructionsRule.RuleFor(instruction => instruction.Step)
                .GreaterThan(0).WithMessage(ResourceMessagesException.NON_NEGATIVE_INSTRUCTION_STEP);
                instructionsRule.RuleFor(instruction => instruction.Text)
                .NotEmpty().WithMessage(ResourceMessagesException.INSTRUCTION_EMPTY)
                .MaximumLength(2000).WithMessage(ResourceMessagesException.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS);
            });

            // Objetivo dessa validação: Não pode haver duas instruções com o mesmo Step.
            RuleFor(recipe => recipe.Instructions).Must(instructions => instructions.Select(i => i.Step).Distinct().Count() == instructions.Count)
                .WithMessage(ResourceMessagesException.TWO_OR_MORE_INSTRUCTIONS_SAME_ORDER);

            // Must -> O .Must é uma regra customizada do FluentValidation.
            // 👉 Ele serve para quando não existe uma validação pronta(como NotEmpty, GreaterThan, etc.).
            // O Must espera um valor true ou false.
            // Distinct -> remove valores duplicados
        }
    }
}
