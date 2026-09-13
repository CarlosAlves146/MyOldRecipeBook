using MyRecipeBook.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRecipeBook.Domain.Entities
{
    public class Recipe : EntityBase
    {
        public string Title {  get; set; } = string.Empty;
        public CookingTime? CookingTime { get; set; } // enum
        public Difficulty? Difficulty { get; set; } // enum
        public IList<Ingredient> Ingredient { get; set; } = [];
        public IList<Instruction> Instructions { get; set; } = [];
        public IList<DishType> DishType { get; set; } = []; // class - enum
        public long UserId { get; set; }        
    }
}
