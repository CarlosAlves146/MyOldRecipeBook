using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extension;
using MyRecipeBook.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace MyRecipeBook.Application.UseCases.User.Update
{
    public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
    {
        public UpdateUserValidator()
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
            RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_EMPTY);

            When(user => string.IsNullOrWhiteSpace(user.Email).IsFalse(), () =>
            {
                RuleFor(user => user.Email)
                    .Must(email =>
                        email.Contains('@') &&
                        !email.Contains(' ') &&
                        new EmailAddressAttribute().IsValid(email))
                    .WithMessage(ResourceMessagesException.EMAIL_INVALID);
            });
        }
    }
}
