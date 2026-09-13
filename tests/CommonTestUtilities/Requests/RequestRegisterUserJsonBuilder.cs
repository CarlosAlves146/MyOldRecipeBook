using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
// Esse arquivo cria um construtor “simplificado” de objetos RequestRegisterUserJson para facilitar testes.
// É como uma "fábrica" que gera o mesmo tipo de objeto toda vez que você chama Build().
// E para que isso aconteça de forma automática adicionamos uma biblioteca chama Bogus
    public class RequestRegisterUserJsonBuilder
    {
        public static RequestRegisterUserJson Build(int passwordLength = 10)
        {
            return new Faker<RequestRegisterUserJson>()
                .RuleFor(user => user.Name, (f) => f.Person.FirstName)
                .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
                .RuleFor(user => user.Password, (f) => f.Internet.Password(passwordLength));       
        }
    }
}
