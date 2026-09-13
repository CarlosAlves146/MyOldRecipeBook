using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Filters;

namespace MyRecipeBook.API.Attributes
{
    // Classe criada para verificar se o usuário está autenticado
    // implementação com : base para anular o erro da obrigatoriedade de repassar um parametro na controller
    // tendo em vista que de não conseguir passar valores de (DI).
    public class AuthenticatedUserAttribute : TypeFilterAttribute
    { 
        // Construtor
        public AuthenticatedUserAttribute() : base(typeof(AuthenticatedUserFilter))
        {
        }
    }
}
