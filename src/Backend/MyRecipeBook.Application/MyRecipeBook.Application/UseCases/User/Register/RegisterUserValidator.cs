using FluentValidation;
using MyRecipeBook.Application.SharedValidators;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extension;
using MyRecipeBook.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
    {
        //Construtor
        public RegisterUserValidator() 
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
            RuleFor(user => user.Name).MaximumLength(255).WithMessage(ResourceMessagesException.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS);
            RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_EMPTY);
            RuleFor(user => user.Email).MaximumLength(255).WithMessage(ResourceMessagesException.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS);
            RuleFor(user => user.Password).SetValidator(new PasswordValidator<RequestRegisterUserJson>());
            RuleFor(user => user.Password).MaximumLength(2000).WithMessage(ResourceMessagesException.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS);

            /*
             Esse bloco valida o e-mail somente se ele NÃO estiver vazio
                Se estiver preenchido, ele precisa:
                - conter @
                - não conter espaços
                - ser considerado válido pelo validador padrão de e-mail do .NET
                Se qualquer uma dessas regras falhar, o e-mail é inválido.
             */
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
