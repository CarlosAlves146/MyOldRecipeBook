namespace MyRecipeBook.Domain.Repositories.User
{
    // Interface reponsável: 
    // por buscar o usuário pelo id
    // proparar o banco para commit
    public interface IUserUpdateOnlyRepository
    {
        public Task<Entities.User> GetById(long id);
        public void Update(Entities.User user);
    }
}
