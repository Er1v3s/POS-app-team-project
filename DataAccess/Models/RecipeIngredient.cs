using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class RecipeIngredient
    {
        [Key]public int RecipeIngredientId {  get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public required double Quantity { get; set; }
    }
}
