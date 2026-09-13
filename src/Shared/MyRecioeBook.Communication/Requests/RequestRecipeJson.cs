using MyRecipeBook.Communication.Enums;

namespace MyRecipeBook.Communication.Requests
{
    public class RequestRecipeJson
    {
        public string Title { get; set; } = string.Empty;
        public CookingTime? CookingTime { get; set; } // enum
        public Difficulty? Difficulty { get; set; } // enum
        public IList<string> Ingredient { get; set; } = [];
        public IList<RequestInstructionJson> Instructions { get; set; } = [];
        public IList<DishType> DishType { get; set; } = []; // enum
    }
}
