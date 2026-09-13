using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestChangePasswordJsonBuilder
    {
        public static RequestChangePasswordJson Build(int lengthPassword = 10)
        {
            return new Faker<RequestChangePasswordJson>()
                .RuleFor(user => user.NewPassword, (f) => f.Internet.Password(lengthPassword));
        }
    }
}
