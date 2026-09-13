using Microsoft.OpenApi.Models;
using MyRecipeBook.API.Converters;
using MyRecipeBook.API.Filters;
using MyRecipeBook.API.Middleware;
using MyRecipeBook.API.Token;
using MyRecipeBook.Application;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.infrastructure;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Migrations;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Converter para requests adicionado.
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new StringConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger configurado para Tokens
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization Header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


// Aqui estamos Configurando o ASP.NET Core para usar o padrão MVC (Model-View-Controller).
// Adicionando um FILTRO GLOBAL chamado ExceptionFilter em todas as requisições da aplicação.
builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

// Configurando a injeção de dependência da aplicação.
// Aqui usamos dois métodos de extensão (AddApplication e AddInfrastructure)
// que servem para organizar e registrar os serviços (dependências) necessários
// em nosso container (builder.Services).
// Lembrando que nesse ponto precisamos realizar os usings de aplication e infrastructure
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration); // Refatorando, agora precisamos passar builder.configuration pq precisamos desse parametro por causa da connectionString
builder.Services.AddScoped<ITokenProvider, HttpContextValue>(); // Relacioando a implementação de leitura de token do usuário para saber onde vamos salvar a receita
// forçando as urls serem minusculas
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddHttpContextAccessor(); // trecho para autorizar a injeção dentro de HttpContextValue

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// add uma etapa p/ cada requisição de identificação e troca de idioma
app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

MigrationDatabase();

// This code below was modified to satify SonarCloud
await app.RunAsync();

void MigrationDatabase()
{
    // verificando se estamos em ambiente de teste
    if (builder.Configuration.IsUnitTestEnvironment())
    {
        return;
    }
    var databaseType = builder.Configuration.DatabaseType();
    var connectionString = builder.Configuration.ConnectionString();

    var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

    DatabaseMigration.Migrate(databaseType, connectionString, serviceScope.ServiceProvider);
}
public partial class Program
{
    // this code below was created to satisfy SonarCloud
    protected Program() { }
}
// Texto adicionado apenas para teste.