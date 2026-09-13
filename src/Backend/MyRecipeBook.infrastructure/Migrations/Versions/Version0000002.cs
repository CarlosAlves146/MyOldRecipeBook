using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_RECIPES, "Create table t save the user`s information")]
    public class Version0000002 : VersionBase
    {
        // Parâmetros de uma Foreign Key (FK)
        // Ao criar uma FK em uma migration, estamos dizendo que uma coluna de uma tabela
        // (tabela filha) está relacionada com outra tabela (tabela pai).
        // Para isso, precisamos informar 3 coisas principais:

        // 1 - Nome da Foreign Key (constraint)
        //     → É apenas um identificador da FK no banco.
        //     → Boa prática: usar um nome que indique claramente a relação.
        //     Ex: FK_Usuarios_Perfis (TabelaUsuarios -> TabelaPerfis)

        // 2 - Nome da tabela principal (tabela pai)
        //     → É a tabela que possui o dado original (a "dona" da informação).
        //     → Ex: "Perfis"

        // 3 - Nome da coluna na tabela principal (chave referenciada)
        //     → Normalmente é a chave primária (Id) da tabela pai.
        //     → É essa coluna que será usada como referência.
        //     Ex: "Id"

        // Resumindo:
        // A FK diz: "Essa coluna da tabela atual só pode ter valores que existem
        // na coluna X da tabela Y".
        public override void Up()
        {
            CreatedTable("Recipes")
                .WithColumn("Title").AsString().NotNullable()
                .WithColumn("CookingTime").AsInt32().Nullable()
                .WithColumn("Difficulty").AsInt32().Nullable()
                .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_Recipe_User_Id", "Users", "Id");

            CreatedTable("Ingredients")
                .WithColumn("Item").AsString().NotNullable()
                .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_Ingredients_Recipe_Id", "Recipes", "Id")
                .OnDelete(System.Data.Rule.Cascade);

            CreatedTable("Instructions")
                .WithColumn("Step").AsInt32().NotNullable()
                .WithColumn("Text").AsString(2000).NotNullable()
                .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_Instruction_Recipe_Id", "Recipes", "Id")
                .OnDelete(System.Data.Rule.Cascade);

            CreatedTable("DishTypes")
                .WithColumn("Type").AsInt32().NotNullable()
                .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_DishType_Recipe_Id", "Recipes", "Id")
                .OnDelete(System.Data.Rule.Cascade);
        }
    }
}
