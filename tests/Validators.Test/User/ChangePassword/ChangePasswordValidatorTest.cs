using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.ChangePassword
{
    public class ChangePasswordValidatorTest
    {
        [Fact]
        public void Success()
        {
            // Arrange -> Prepara
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build();

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.ShouldNotBeNull();
            result.IsValid.ShouldBeTrue();  
        }

        [Fact]
        public void Error_NewPasssword_Empty()
        {
            // Arrange -> Prepara
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build();
            // Invalidando a NewPassword
            request.NewPassword = string.Empty;

            // Act -> Agir/Executar
            var result = validator.Validate(request); 

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();

            result.Errors.ShouldHaveSingleItem();

            var error = result.Errors.Single();
           
            // Propriedade onde foi o erro
            error.PropertyName.ShouldBe("NewPassword");

            error.ErrorMessage.ShouldBe(ResourceMessagesException.PASSWORD_EMPTY);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Error_Passsword_Invalid(int passwordLength)
        {
            // Arrange -> Prepara
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build(passwordLength);

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();

            var error = result.Errors.Single();
            error.PropertyName.ShouldBe("NewPassword");
            error.ErrorMessage.ShouldBe(ResourceMessagesException.PASSWORD_NUMBER_DIGITS_BELOW);
        }
    }
}
