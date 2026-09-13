using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.infrastructure.DataAccess;

// Em nossa class estamos herdando de DbContext que é a class base do Entity framework Core
public class MyRecipeBookDbContext : DbContext
{
    // Construtor
    // Estamos recebendo em nosso construtor um parametro que contêm nossas configurações do BD
    // Esse DbContextOption é necessário para o Entity Framework saber como se conectar e operar com o banco
    // Essa configuração foi feita através do método de EXTENSÃO.
    // * GERALMENTE ESSE OPTIONS É CONFIGURADO EM PROGRAM.CS
    // : base(options) Esse trecho, estamos passando essa variável para a classe PAI (DbContext).
    public MyRecipeBookDbContext(DbContextOptions options) : base(options) 
    {
    } 

    // Propriedade DbSet<user>
    // ela representa uma tabela do banco de dados.
    // Nesse caso User é uma entidade, do nosso "Domain" uma classe que representa um registro/tabela
    // está dizendo... Tenho uma tabela chamada Users no meu BD e vou manipulá-la aqui.
    public DbSet<User> Users { get; set; }
    public DbSet<Recipe> Recipes { get; set; }


    // Esse método é chamado automaticamente quando o EF está montando o modelo do banco de dados.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyRecipeBookDbContext).Assembly);
    }
}
