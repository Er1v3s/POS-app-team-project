namespace POS.Models
{
    public class Products
    {
        public int Product_id { get; set; }
        public required string Product_name { get; set; }
        public required string Category { get; set; }
        public string? Description { get; set; }
        public required double? Price { get; set; }
        public required int Recipe_id { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
