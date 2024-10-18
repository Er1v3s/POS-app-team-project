using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class RecipeIngredients
    {
        [Key]public int RecipeIngredientId {  get; set; }
        public int RecipeId { get; set; }
        public Recipes Recipe { get; set; }
        public int IngredientId { get; set; }
        public Ingredients Ingredient { get; set; }
        public required double Quantity { get; set; }
    }
}
