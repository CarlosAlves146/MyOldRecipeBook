using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Domain.Repositories.User
{
    public interface IChangePasswordUseCase
    {
        public Task Execute(RequestChangePasswordJson request);
    }
}
