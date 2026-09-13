using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.infrastructure.DataAccess.Repositories;

// Nossa Class UserRepository:
// É uma classe que serve como "repositório", ou seja, é responsável por fazer
// a comunicação direta com o banco de dados.
// Estamos implementando métodos que estão definidos nas duas interfaces.
public class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository
{
  
    private readonly MyRecipeBookDbContext _dbContext;

    // Construtor:
    // Ele recebe um parâmetro do tipo "MyRecipeBookDbContext"
    // e atribuimos esse valor a _dbcontext
    // Assim toda vez que a classe UserRepository for criada, ela obrigatoriamente recebe uma
    // conexão com o bando de dados.
    public UserRepository(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Quando possivel sempre utilize método assincronos/async Task
    // Nosso método para adicionar um Usuário.
    // O método recebe um parâmetro do tipo "User", que é nossa Entidade de domínio. 
    // Lembrando que aqui estamos cumprindo o contrato das interfaces IUserWriteOnlyRepository

    public async Task Add(User user)
    {
        // Nesse trecho Acessamos a tabela "Users" do nosso banco de dados e adicionando a ela nosso
        // Objeto "user"
        await _dbContext.Users.AddAsync(user);
    }
    // Método que verifica se há um e-mail já cadastrado
    public async Task<bool> ExistActiveUserWithEmail(string email)
    {
        // Aqui vamos verificar se há um email igual o informado já cadastrado e se ele está ativo
       return await _dbContext.Users.AnyAsync(user => user.Email.Equals(email) && user.Active);
    }

    public async Task<User?> GetByEmailAndPassword(string email, string password)
    {
        return await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email.Equals(email) && user.Password.Equals(password));
    }

    // Método que verifica se há usuário "Identifier" correspondente e "ativo"  - 102
    public async Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier)
    {
        return await _dbContext
            .Users
            .AnyAsync(user => user.UserIdentifier.Equals(userIdentifier) && user.Active);
    }

    // Retornando o Usuário com o Identifier?
    public async Task<User?> GetByUserIdentifier(Guid userIdentifier)
    {
        return await _dbContext
            .Users
            .AsNoTracking()
            .FirstAsync(user => user.Active && user.UserIdentifier.Equals(userIdentifier));
    }

    public async Task<User> GetById(long id)
    {
        return await _dbContext
            .Users
            .FirstAsync(user => user.Id == id);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }
}
