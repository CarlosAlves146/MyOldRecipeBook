using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extension;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.infrastructure.DataAccess;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories
{
    public sealed class RecipeRepository : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository
    {
        private readonly MyRecipeBookDbContext _dbContext;

        // Construtor:
        // Ele recebe um parâmetro do tipo "MyRecipeBookDbContext"
        // e atribuimos esse valor a _dbcontext
        // Assim toda vez que a classe UserRepository for criada, ela obrigatoriamente recebe uma
        // conexão com o bando de dados.
        public RecipeRepository(MyRecipeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Recipe recipe)
        {
            // Nesse trecho Acessamos a tabela "Recipes" do nosso banco de dados e adicionando a ela nosso
            // Objeto "recipe"
            await _dbContext.Recipes.AddAsync(recipe);
        }
        public async Task<IList<Recipe>> Filter(User user, FilterRecipesDto filters)
        {
            var query = _dbContext
                .Recipes
                .AsNoTracking()
                .Include(recipe => recipe.Ingredient)
                .Where(recipe => recipe.Active && recipe.UserId == user.Id);              
            

            if(filters.Difficulties.Any())
            {
                // Para cada receita, a lista de dificuldades do filtro contém a dificuldade desta receita?
                query = query.Where(recipe => recipe.Difficulty.HasValue && filters.Difficulties.Contains(recipe.Difficulty.Value));
            }
            
            if (filters.CookingTimes.Any())
            {
                // Para cada receita, a lista de CookingTime do filtro contém o CookingTime desta receita?
                query = query.Where(recipe => recipe.CookingTime.HasValue && filters.CookingTimes.Contains(recipe.CookingTime.Value));
            }

            if (filters.DishTypes.Any())
            {
                // dishType representa cada tipo de prato da receita durante a verificação do Any() *interno ao if.
                query = query.Where(recipe => recipe.DishType.Any(dishType => filters.DishTypes.Contains(dishType.Type)));
            }

            if (filters.RecipeTitle_Ingredient.NotEmpty())
            {
                // Para cada receita, verifica se o texto pesquisado está no título ou em algum ingrediente.
                query = query.Where(recipe => recipe.Title.Contains(filters.RecipeTitle_Ingredient) 
                || recipe.Ingredient.Any(ingredient => ingredient.Item.Contains(filters.RecipeTitle_Ingredient)));
            }
            
            return await query.ToListAsync();
        }

    }
}
