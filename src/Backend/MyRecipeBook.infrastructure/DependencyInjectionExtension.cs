using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Encrypt;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.infrastructure.DataAccess;
using MyRecipeBook.infrastructure.DataAccess.Repositories;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.DataAccess.Repositories;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Security.Cryptography;
using MyRecipeBook.Infrastructure.Security.Tokens.Access.Generator;
using MyRecipeBook.Infrastructure.Security.Tokens.Access.Validator;
using MyRecipeBook.Infrastructure.Services.LoggedUser;
using System.Reflection;

namespace MyRecipeBook.infrastructure
{
    // Classe com método de extensão responsável por configurar a camada de infraestrutura da aplicação
    // podendo escolher entre a implementação de depois tipos de banco de dados.
    // - AddDbContext_SqlServer(): 
    // - AddDbContext_MySqlServer(): 
    // - AddRepositories(): Registra os repositórios no container de injeção de dependência.
    // - Refatorando o código agora no método adicionamos mais um parâmetro para configurar nossa 
    // ConnectionString, IConfiguration configuration.

    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddRepositories(services);
            AddPasswordEncrypter(services, configuration);
            AddLoggedUser(services);
            AddTokens(services, configuration);

            // verificando se estamos em ambiente de teste em app.settings.Test
            if (configuration.IsUnitTestEnvironment())
            {
                return;
            }
            // Recuperando de appsettings.Development o valor de "DataBaseType"
            // var databaseType = configuration.GetConnectionString("DataBaseType"); Aula 56, esse trecho foi adicionado no método de extensão de IConfiguration de Migrate

            // Convertendo o valor de "DataBaseType" para um valor de Enum
            // Aqui nesse trecho, precisamos utilizar o "cast" pq quando convertemos, databaseType para DatabaseType
            // o Enum.Parse, ele nos retorna um object ai precisamos converter esse object para o tipo certo,
            // que é um "DatabaseType" lá do nosso Enum, e para isso fazemos um cast.
            // "Converta a string "0" em um valor do tipo DatabaseType, ou seja, no enum correspondente."
            // var databaseTypeEnum = Enum.Parse<DatabaseType>(databaseType!); versão moderna sem precisar usar o cast, ainda não testei
            //var databaseTypeEnum = (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType!);

            // chamando nosso método de extensão
            var databaseType = configuration.DatabaseType();
            
            if (databaseType == DatabaseType.SqlServer)
            {
                AddDbContext_SqlServer(services, configuration);
                AddFluentMigrator_SqlServer(services, configuration);
            }
            else if (databaseType == DatabaseType.MySql)
            {
                AddDbContext_MYSQLServer(services, configuration);
                AddFluentMigrator_MySql(services, configuration);
            }
        }
        private static void AddDbContext_SqlServer(IServiceCollection services, IConfiguration configuration)
        {
            // chamando nosso método de extensão
            var connectionString = configuration.ConnectionString();

            // Toda vez que alguém precisar de um MyRecipeBookDbContext, crie um para mim usando essa configuração aqui.
            services.AddDbContext<MyRecipeBookDbContext>(dbContextOptions => 
            {
                dbContextOptions.UseSqlServer(connectionString);
            });
        }
        private static void AddDbContext_MYSQLServer(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();

            // Toda vez que alguém precisar de um MyRecipeBookDbContext, crie um para mim usando essa configuração aqui.
            services.AddDbContext<MyRecipeBookDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(connectionString);
            });
        }
        private static void AddRepositories(IServiceCollection services)
        {
            // Esse comando está dizendo:
            // — "Sempre que alguém pedir um IUserWriteOnlyRepository, entrega para ele uma instância da classe UserRepository."
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
            services.AddScoped<IRecipeWriteOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeReadOnlyRepository, RecipeRepository>();
            
        }
        private static void AddFluentMigrator_MySql(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options
                .AddMySql5()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
            });
        }
        private static void AddFluentMigrator_SqlServer(IServiceCollection services, IConfiguration configuration)
        {
            // Método responsável por configurar o FluentMigrator para o SQL Server.
            // 1. Obtém a string de conexão do arquivo de configuração (appsettings.json).
            // 2. Registra o FluentMigrator no container de injeção de dependência.
            // 3. Define o banco de dados usado (SQL Server) e a string de conexão global.
            // 4. Faz o FluentMigrator procurar todas as classes de migração dentro do assembly "MyRecipeBook.Infrastructure".
            // 5. Configura o log para exibir no console as operações realizadas durante as migrações.

            var connectionString = configuration.ConnectionString();
            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
            });
            services.AddFluentMigratorCore().AddLogging(lb =>
            {
                lb.AddFluentMigratorConsole();
            });
        }
        private static void AddTokens(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");
            
            services.AddScoped<IAccessTokenGenerator>(option => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
            services.AddScoped<IAccessTokenValidator>(option => new JwtTokenValidator(signingKey!));
        }
        private static void AddLoggedUser(IServiceCollection services)
        {
            services.AddScoped<ILoggedUser, LoggedUser>();
        }
        private static void AddPasswordEncrypter(IServiceCollection services, IConfiguration configuration)
        {
            // Recuperando nossa adicional key de Appsettings
            var additionalKey = configuration.GetValue<string>("Settings:Password:AddtionalKey");

            services.AddScoped<IPasswordEncrypter>(option => new Sha512Encripter(additionalKey!));
        }
    }
}
