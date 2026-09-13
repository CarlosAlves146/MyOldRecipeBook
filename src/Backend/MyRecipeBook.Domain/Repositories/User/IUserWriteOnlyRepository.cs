namespace MyRecipeBook.Domain.Repositories.User
{
    public interface IUserWriteOnlyRepository
    {
        // No parâmetro precisamos especificar o caminho Entities.User por que está no mesmo projeto
        // e somente User, não estava localizando.
        public Task Add(Entities.User user);
    }
}
