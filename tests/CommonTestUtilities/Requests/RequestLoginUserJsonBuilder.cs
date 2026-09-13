using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestLoginUserJsonBuilder
    {
        public static RequestLoginJson Build(int passwordLength = 10)
        {
            
            return new Faker<RequestLoginJson>()
                .RuleFor(user => user.Email, (f) => f.Internet.Email())
                .RuleFor(user => user.Password, (f) => f.Internet.Password(passwordLength));
        }
    }
}
