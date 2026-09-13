using MyRecipeBook.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRecipeBook.Domain.Entities
{
    [Table("DishTypes")]
    public class DishType : EntityBase
    {
        public Enums.DishType Type { get; set; } // Especificação de caminho necessário por causa do nome da classe 05:30 - 116
        public long RecipeId { get; set; }
    }
}
