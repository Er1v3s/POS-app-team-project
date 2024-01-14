using System.Collections.Generic;

namespace POS.Models
{
    public class Ingredients
    {
        public int Ingredient_id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string? Unit { get; set; }
        public string? Expiration_date {  get; set; }
        public string? Package {  get; set; }
        public int? Stock {  get; set; }
        public int Safety_stock { get; set; }
        public ICollection<RecipeIngredients> RecipeIngredients { get; set; }
    }
}
