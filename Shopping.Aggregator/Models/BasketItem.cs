namespace Shopping.Aggregator.Models
{
    public class BasketItem
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
