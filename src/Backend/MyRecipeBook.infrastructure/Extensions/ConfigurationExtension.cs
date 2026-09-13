using Microsoft.Extensions.Configuration;
using MyRecipeBook.Domain.Enums;
using System.Diagnostics.Eventing.Reader;

namespace MyRecipeBook.Infrastructure.Extensions
{
    public static class ConfigurationExtension
    {
        // Método verifica se estamos em ambiente de "Test"
        public static bool IsUnitTestEnvironment(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("InMemoryTest");
        }
        public static DatabaseType DatabaseType(this IConfiguration configuration)
        {
            // Recuperando o databaseType do appsettings
            var databaseType = configuration.GetConnectionString("DataBaseType");

            // retornando o tipo do databaseType. Convertido para DatabaseType de Enum
            return (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType!);
        }
        public static string ConnectionString(this IConfiguration configuration)
        {
            // Aqui tbm temos acesso a databaseType por tbm ser um método de extensão de IConfiguration.
            var databaseType = configuration.DatabaseType();

            if (databaseType == Domain.Enums.DatabaseType.MySql)
            {
                return configuration.GetConnectionString("ConnectionMySQLServer")!;
            }
            else
            {
                return configuration.GetConnectionString("ConnectionSQLServer")!;
            }
        }
    }
}
