namespace DataAccess.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string Category { get; set; }
        public string Description { get; set; }
        public required double Price { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
