namespace Shopping.Aggregator.Models
{
    public class BasketModel
    {
        public string UserName { get; set; } = default!;
        public List<BasketItem> Items { get; set; } = new();
    }
}
