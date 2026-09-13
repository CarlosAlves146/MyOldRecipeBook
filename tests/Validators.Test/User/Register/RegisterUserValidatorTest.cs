using CommonTestUtilities.Requests;
using MyRecipeBook.Application.SharedValidators;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.Register
{
    public class RegisterUserValidatorTest
    {
        [Fact]
        public void Sucess()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            // Isso garante que o “resultado da validação” veio certinho e não está nulo.
            result.ShouldNotBeNull();

            // “Como essa Request foi preenchida corretamente pelo Builder, o resultado tem que ser verdadeiro.”
            // Se der false → o teste falha.
            result.IsValid.ShouldBeTrue();

            // “Como está tudo certo, a lista de erros tem que estar vazia.”
            result.Errors.ShouldBeEmpty();

            // Validação das propriedades geradas pelo Builder/Bogus
            request.Name.ShouldNotBeNullOrEmpty();
            request.Email.ShouldNotBeNullOrEmpty();
            request.Password.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Error_Name_Empty()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty; // forçando uma falha no Name

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            // Isso garante que o “resultado da validação” veio certinho e não está nulo. E de acordo com nossa modificação no Name geramos um erro
            result.ShouldNotBeNull();

            // “Como essa Request foi preenchida corretamente pelo Builder, o resultado deveria ser verdadeiro. Porem na linha 80 forçamos uma falha
            // deixando a propriedade Name, vazia”
            // Ou seja essa verificação abaixo realmente será true → por que de fato esperamos uma falha.
            result.IsValid.ShouldBeFalse();

            // Deve haver exatamente 1 erro
            // Pense assim:
            // “Validator, se deu erro, era pra ter só um erro. Nem mais, nem menos.”
            result.Errors.ShouldHaveSingleItem();

            // Pega o único erro da lista, Se tivesse mais de um erro → o teste falharia nessa linha.
            var error = result.Errors.Single();

            // Isso confirma que o erro veio exatamente da Propriedade Name. Se aparecesse erro em outro propriedade (como Email), o teste falha.
            error.PropertyName.ShouldBe("Name");

            // A mensagem deve ser exatamente a definida no recurso
            // Aqui você compara a mensagem real com a mensagem que você MANDOU o validator usar. Se o validator estiver usando outra mensagem → teste falha
            error.ErrorMessage.ShouldBe(ResourceMessagesException.NAME_EMPTY);

            // Validação das propriedades geradas pelo Builder/Bogus - E nossa falha inserida no Name.
            request.Name.ShouldBeEmpty();
            request.Email.ShouldNotBeNullOrEmpty();
            request.Password.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Error_Email_Empty()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = string.Empty; // forçando uma falha no Email

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            // Isso garante que o “resultado da validação” veio certinho e não está nulo.
            result.ShouldNotBeNull();

            // “Como essa Request foi preenchida corretamente pelo Builder, o resultado tem que ser verdadeiro. Porem na linha 121 forçamos uma falha
            // deixando a propriedade Name, vazia”
            // Ou seja essa verificação abaixo realmente será true → por que de fato esperamos uma falha.
            result.IsValid.ShouldBeFalse();

            // Deve haver exatamente 1 erro
            // Pense assim:
            // “Validator, se deu erro, era pra ter só um erro. Nem mais, nem menos.”
            result.Errors.ShouldHaveSingleItem();

            // Pega o único erro da lista, Se tivesse mais de um erro → o teste falharia nessa linha.
            var error = result.Errors.Single();

            // Isso confirma que o erro veio exatamente do campo Name. Se aparecesse erro em outro campo (como Name), o teste falha.
            error.PropertyName.ShouldBe("Email");

            // A mensagem deve ser exatamente a definida no recurso
            // Aqui você compara a mensagem real com a mensagem que você MANDOU o validator usar. Se o validator estiver usando outra mensagem → teste falha
            error.ErrorMessage.ShouldBe(ResourceMessagesException.EMAIL_EMPTY);

            // Validação das propriedades geradas pelo Builder/Bogus - E nossa falha inserida no Email.
            request.Name.ShouldNotBeNullOrEmpty();
            request.Email.ShouldBeEmpty();
            request.Password.ShouldNotBeNullOrEmpty();
        }


        [Fact]
        public void Error_Email_Invalid()
        {
            // Arrange -> Prepara/Organizar
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = "email .com"; // forçando uma falha no Email
            
            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            // Isso garante que o “resultado da validação” veio certinho e não está nulo.
            result.ShouldNotBeNull();

            // “Como essa Request foi preenchida corretamente pelo Builder, o resultado tem que ser verdadeiro. Porem na linha 162 forçamos uma falha
            // deixando a propriedade Name, vazia”
            // Ou seja essa verificação abaixo realmente será true → por que de fato esperamos uma falha.
            result.IsValid.ShouldBeFalse();

            // Deve haver exatamente 1 erro
            // Pense assim:
            // “Validator, se deu erro, era pra ter só um erro. Nem mais, nem menos.”
            result.Errors.ShouldHaveSingleItem();

            // Pega o único erro da lista, Se tivesse mais de um erro → o teste falharia nessa linha.
            var error = result.Errors.Single();

            // Isso confirma que o erro veio exatamente do campo Email. Se aparecesse erro em outro campo (como Name), o teste falha.
            error.PropertyName.ShouldBe("Email");
            
            // A mensagem deve ser exatamente a definida no recurso
            // Aqui você compara a mensagem real com a mensagem que você MANDOU o validator usar. Se o validator estiver usando outra mensagem → teste falha
            error.ErrorMessage.ShouldBe(ResourceMessagesException.EMAIL_INVALID);

            // Validação das propriedades geradas pelo Builder/Bogus - E nossa falha inserida no Email.
            request.Name.ShouldNotBeNullOrEmpty();
            request.Email.ShouldNotBeNullOrEmpty();
            request.Password.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Error_Password_Empty()
        {
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Password = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

            var error = result.Errors.Single();

            error.PropertyName.ShouldBe("Password");

            error.ErrorMessage.ShouldBe(ResourceMessagesException.PASSWORD_EMPTY);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Error_Password_Invalid(int passwordLength)
        {
            // Arrange -> Prepara/Organizar
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build(passwordLength);
            

            // Act -> Agir/Executar
            var result = validator.Validate(request);

            // Assert -> Verificar/Validar
            // Isso garante que o “resultado da validação” veio certinho e não está nulo.
            result.ShouldNotBeNull();

            // “Como essa Request foi preenchida corretamente pelo Builder, o resultado tem que ser verdadeiro. Porem estamos forçando pelo parametro apeans quantidades de caracteres
            // inferior ao exigido que são 6.
            // Ou seja essa verificação abaixo realmente será true → por que de fato esperamos uma falha.
            result.IsValid.ShouldBeFalse();

            // Deve haver exatamente 1 erro
            result.Errors.ShouldHaveSingleItem();

            // Pega o único erro da lista, Se tivesse mais de um erro → o teste falharia nessa linha.
            var error = result.Errors.Single();

            // Isso confirma que o erro veio exatamente do campo Password.
            error.PropertyName.ShouldBe("Password");

            // A mensagem deve ser exatamente a definida no recurso
            // Aqui você compara a mensagem real com a mensagem que você MANDOU o validator usar. Se o validator estiver usando outra mensagem → teste falha
            error.ErrorMessage.ShouldBe(ResourceMessagesException.PASSWORD_NUMBER_DIGITS_BELOW);
            

            // Validação das propriedades geradas pelo Builder/Bogus - E nossa falha inserida no Email.
            request.Name.ShouldNotBeNullOrEmpty();
            request.Email.ShouldNotBeNullOrEmpty();
            request.Password.ShouldNotBeNull();
        }
    }
}
