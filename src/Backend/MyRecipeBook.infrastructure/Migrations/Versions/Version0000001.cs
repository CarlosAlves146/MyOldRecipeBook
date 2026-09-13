using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    // A implementação que de fato tem a versão, e uma descrição sobre ela.
    [Migration(DatabaseVersions.TABLE_USER,"Create table t save the user`s information")]
    public class Version0000001 : VersionBase
    {
        public override void Up()
        {
                CreatedTable("Users")
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Email").AsString(255).NotNullable()
                .WithColumn("Password").AsString(2000).NotNullable()
                .WithColumn("UserIdentifier").AsGuid().NotNullable();
        }         
    }
}
