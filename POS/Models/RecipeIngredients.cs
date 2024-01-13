using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class RecipeIngredients
    {
        [Key]public int RecipeIngredient_id {  get; set; }
        public int Recipe_id { get; set; }
        public Recipes Recipe { get; set; }
        public int Ingredient_id { get; set; }
        public Ingredients Ingredient { get; set; }
        public required double Quantity { get; set; }
    }
}
