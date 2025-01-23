namespace DataAccess.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string? Unit { get; set; }
        public string? ExpirationDate {  get; set; }
        public string? Package {  get; set; }
        public int? Stock {  get; set; }
        public int SafetyStock { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
