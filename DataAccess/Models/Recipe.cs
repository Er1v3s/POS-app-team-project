namespace DataAccess.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public required string RecipeName { get; set; }
        public required string RecipeContent {  get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}