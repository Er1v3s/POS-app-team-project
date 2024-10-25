using System.Collections.Generic;

namespace POS.Models
{
    internal class Ingredients
    {
        public int IngredientId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string? Unit { get; set; }
        public string? ExpirationDate {  get; set; }
        public string? Package {  get; set; }
        public int? Stock {  get; set; }
        public int SafetyStock { get; set; }
        public ICollection<RecipeIngredients> RecipeIngredients { get; set; }
    }
}
