using Moq;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Entities;

namespace CommonTestUtilities.Repositories
{
    public class UserReadOnlyRepositoryBuilder
    {
        // Aqui criamos um campo privado que guarda o "mock".
        // Um *mock* é um objeto FAKE que imita uma interface,
        // mas sem acessar banco de dados de verdade.
        private readonly Mock<IUserReadOnlyRepository> _repository;

        public UserReadOnlyRepositoryBuilder()
        {
            // Quando criamos um novo Builder, já iniciamos um Mock da interface.
            // É como dizer: "Me dá um repositório falso para eu configurar nos meus testes".
            _repository = new Mock<IUserReadOnlyRepository>();
        }

        public void ExistActiveUserWithEmail(string email)
        {
            // Aqui configuramos o mock:
            // Quando o método ExistActiveUserWithEmail(email) for chamado no teste,
            // ele deve retornar TRUE (de forma assíncrona).
            //
            // Em outras palavras:
            // "Mock, toda vez que alguém perguntar se existe um usuário com esse e-mail,
            // responda que SIM."
            //
            // Isso é usado quando queremos simular um cenário em que o e-mail já está cadastrado.
            _repository
                .Setup(repository => repository.ExistActiveUserWithEmail(email))
                .ReturnsAsync(true);
        }
        public void GetByEmailAndPassword(User user)
        {
            // Configura o comportamento do Mock:
            // quando o método GetByEmailAndPassword for chamado com o mesmo
            // Email e Password do user informado, o repositório retornará esse user.
            // Isso simula a busca de um usuário válido no banco de dados.
            _repository
                 .Setup(repository => repository.GetByEmailAndPassword(user.Email, user.Password))
                .ReturnsAsync(user);
        }

        public IUserReadOnlyRepository Build()
        {
            // Aqui devolvemos o OBJETO FAKE já configurado.
            // Esse é o que o useCase vai receber no teste.
            //
            // Importante:
            // mock.Object é o objeto que implementa a interface de verdade,
            // só que sem lógica real — apenas o que foi configurado no Setup().
            return _repository.Object;
        }
    }

}
