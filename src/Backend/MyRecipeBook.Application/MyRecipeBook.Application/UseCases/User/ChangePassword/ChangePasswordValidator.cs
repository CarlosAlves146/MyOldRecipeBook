using FluentValidation;
using MyRecipeBook.Application.SharedValidators;
using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword
{
    public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
    {
        public ChangePasswordValidator()
        {
            RuleFor(user => user.NewPassword).SetValidator(new PasswordValidator<RequestChangePasswordJson>());
        }
    }
}
