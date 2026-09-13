using CommonTestUtilities.Requests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.infrastructure.DataAccess;

namespace WebApi.Test
{
    // Classe que estende o comportamento de "WebApplicationFactory"
    // Classe criada para customizar alguns comportamentos do servidor
    // E permite subir a API inteira em memória.
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        // variáveis criadas "privadas" para acessa-las vamos criar 2 funções publicas
        private MyRecipeBook.Domain.Entities.User _user = default!;
        private string _password = String.Empty;
        

        // Método responsável por configurar como nossa API é criada em testes.
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // permite modificar os serviços injetados via DI (Dependency Injection) 
            builder.UseEnvironment("Test")
                .ConfigureServices(services =>
                {
                    // Aqui nós estamos procurando dentro da lista de serviços do ASP.NET Core um serviço já registrado anteriormente.
                    // E esse serviço que estamos procurando é o DbContextOptions se ele achar irá retorna-lo, caso não ache retorna Null.
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MyRecipeBookDbContext>));

                    // Verificando se achou o serviço, se sim, vamos remove-lo
                    if (descriptor is not null)
                    {
                        // Esse código é responsável por remover do container de injeção de dependência (DI) um serviço específico que já
                        // estava registrado anteriormente — no caso, o serviço que contém o DbContextOptions<MyRecipeBookDbContext>, que foi localizado na parte anterior:
                        services.Remove(descriptor);
                    }

                    // Adicionando o banco InMemory e criando o ServiceProvider para gerar o DbContext
                    var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
                    
                    services.AddDbContext<MyRecipeBookDbContext>(options =>
                    {
                        // Aqui estamos dizendo para o Entity Framework usar um banco de dados "IN MEMORY".
                        // Isso significa que NÃO será criado um banco real no SQL Server.
                        // Os dados ficam somente na memória do computador enquanto o teste está rodando.
                        // Quando o teste termina, tudo é apagado automaticamente.
                        options.UseInMemoryDatabase("InMemoryDbForTesting");

                        // Aqui estamos informando ao EF qual "provedor interno" ele deve usar.
                        // Esse "provider" foi criado anteriormente com AddEntityFrameworkInMemoryDatabase().
                        // Ele garante que a configuração do banco em memória seja usada corretamente
                        // e evita conflitos com outras configurações de banco que existam no projeto real.
                        options.UseInternalServiceProvider(provider);
                    });

                    using var scope = services.BuildServiceProvider().CreateScope();

                    // Recuperando nosso dbContext
                    var dbContext = scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();

                    // Garantindo que a base de dados vai começar vazia.
                    dbContext.Database.EnsureDeleted();
                                     
                    // Por questões de organização deixamos esse método que adiciona o usuário fora.
                    StartDatabase(dbContext);
                });
        }
        public string GetEmail()
        {
            return _user.Email;
        }
        public string GetPassword()
        {
            // função que retorna a password antes de ser criptografada
            return _password;
        }
        public string GetName()
        {
            return _user.Name;
        }
        public Guid GetUserIdentifier()
        {
            return _user.UserIdentifier;
        }

        // Adicionando um usuário ao banco em memória
        private void StartDatabase(MyRecipeBookDbContext dbContext)
        {
            // Aqui vamos utilizar o Builder de usuário que criamos
            // Salvando o retorno na variáveis privadas
            (_user, _password) = UserBuilder.Build();

            dbContext.Users.Add(_user); 
            dbContext.SaveChanges();
        }
    }
}
