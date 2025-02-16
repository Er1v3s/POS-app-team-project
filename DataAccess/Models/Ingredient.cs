namespace DataAccess.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; } = "";
        public string Package { get; set; } = "";
        public int Stock { get; set; } = 0;
        public int SafetyStock { get; set; } = 0;
        public string? ExpirationDate {  get; set; }
    }
}