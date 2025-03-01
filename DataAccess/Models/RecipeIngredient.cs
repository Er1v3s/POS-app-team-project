namespace DataAccess.Models
{
    public class RecipeIngredient
    {
        public int RecipeIngredientId {  get; set; }
        public int RecipeId { get; set; }
        public required Recipe Recipe { get; set; }
        public int IngredientId { get; set; }
        public required Ingredient Ingredient { get; set; }
        public required double Quantity { get; set; }
    }
}
