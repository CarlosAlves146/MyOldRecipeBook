namespace MyRecipeBook.Domain.Extension
{
    public static class BooleanExtension
    {
        public static bool IsFalse(this bool value)
        {
            return !value;
        }
    }
}
