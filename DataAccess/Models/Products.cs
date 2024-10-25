namespace POS.Models
{
    internal class Products
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string Category { get; set; }
        public string? Description { get; set; }
        public required double? Price { get; set; }
        public required int RecipeId { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
