using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.Update
{
    public class UpdateUserValidatorTest
    {
        [Fact]
        public void Success()
        {
            // Arrange -> Prepara
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
        }
        [Fact]
        public void Error_Name_Empty()
        {
            // Arrange -> Prepara
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            var error = result.Errors.Single();
            error.PropertyName.ShouldBe("Name");
            error.ErrorMessage.ShouldBe(ResourceMessagesException.NAME_EMPTY);
        }
        [Fact]
        public void Error_Email_Empty()
        {
            // Arrange -> Prepara
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = string.Empty;

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            var error = result.Errors.Single();
            error.PropertyName.ShouldBe("Email");
            error.ErrorMessage.ShouldBe(ResourceMessagesException.EMAIL_EMPTY);
        }
        [Fact]
        public void Error_Email_Invalid()
        {
            // Arrange -> Prepara
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = "invalid.com";

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            var error = result.Errors.Single();
            error.PropertyName.ShouldBe("Email");
            error.ErrorMessage.ShouldBe(ResourceMessagesException.EMAIL_INVALID);
        }
    }
}
