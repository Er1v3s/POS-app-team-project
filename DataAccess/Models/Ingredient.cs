namespace DataAccess.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Unit { get; set; }
        public required string Package { get; set; }
        public int Stock { get; set; } = 0;
        public int SafetyStock { get; set; } = 0;
        public string? ExpirationDate {  get; set; }
    }
}