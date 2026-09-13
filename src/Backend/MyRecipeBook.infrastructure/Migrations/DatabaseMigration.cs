using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extension;
using MySql.Data.MySqlClient;

namespace MyRecipeBook.Infrastructure.Migrations
{
    // Class, reponsável por criar nossos Schemmas e tabelas.
    public static class DatabaseMigration
    {
        // Método q recebe o tipo do banco de dados, e a string de conexão que vem de appsettings
        public static void Migrate(DatabaseType database, string connectionString, IServiceProvider serviceProvider)
        {
            // Identificando qual banco de dados appsetings está trabalhando
            if (database == DatabaseType.MySql)
            {
                EnsureDatabaseCreated_MySql(connectionString);
            }
            else
            {
                EnsureDatabaseCreated_SqlServer(connectionString);
            }
            MigrationDatabase(serviceProvider);
        }
        private static void EnsureDatabaseCreated_MySql(string connectionString) 
        { 
            // Class do próprio DotNet, que contém a connection string do appsettings
            var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);

            // Recuperando apenas o nome do Database.
            var dataBaseName = connectionStringBuilder.Database;

            // Removendo momentaneamente o nome do dataBase
            connectionStringBuilder.Remove("Database");

            // Conexão apenas com o servidor, após remover o nome do database
            using var dbConnection = new MySqlConnection(connectionStringBuilder.ConnectionString);

            // Acrescentando o nome do Database que estamos procurando na nossa Query.
            var parameters = new DynamicParameters();
            parameters.Add("name", dataBaseName);


            var records = dbConnection.Query("SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name", parameters);

            // Se falso
            if (records.Any().IsFalse())
            {
                dbConnection.Execute($"CREATE DATABASE {dataBaseName}");
            }
        }
        private static void EnsureDatabaseCreated_SqlServer(string connectionString)
        {
            // Class do próprio DotNet, que contém a connection string do appsettings
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            // Recuperando apenas o nome do Database.
            var dataBaseName = connectionStringBuilder.InitialCatalog;

            // Removendo momentaneamente o nome do dataBase/Initial Catalog, podemos até deixar dataBase mesmo que ele entende.
            connectionStringBuilder.Remove("Database");

            // Conexão apenas com o servidor.
            using var dbConnection = new SqlConnection(connectionStringBuilder.ConnectionString);

            // Acrescentando o nome do Database que estamos procurando na nossa Query.
            var parameters = new DynamicParameters();
            parameters.Add("name", dataBaseName);


            var records = dbConnection.Query("SELECT * FROM sys.databases WHERE name = @name", parameters);

            // Se falso
            if (records.Any().IsFalse())
            {
                // Utilizando método de extensão que foi incluido pelo Dapper dentro do 
                dbConnection.Execute($"CREATE DATABASE {dataBaseName}");
            }

        }

        private static void MigrationDatabase(IServiceProvider serviceProvider)
        {
            // Aqui estamos atribuindo esse serviço a variável runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            runner.ListMigrations(); // listar todas as nossas migrations/versões

            runner.MigrateUp(); // Controle de comunicar com o banco e executar oque precisa ser mudado.
        }
    }
}
