using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    // Lembrando que a classe ForwardOnlyMigration é abstrata e quando eu faço uma herança dela
    // em VersionBase que também é abstrata, eu passo a responsabilidade de implementação do método UP
    // Para a classe que herdar de VersionBase que não é abstrata, no caso a Version0000001.
    public abstract class VersionBase : ForwardOnlyMigration
    {
        protected ICreateTableColumnOptionOrWithColumnSyntax CreatedTable(string table)
        {
            return Create.Table(table)
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Active").AsBoolean().NotNullable()
                .WithColumn("CreatedOn").AsDateTime().NotNullable();
        }
    }
}
