using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services
{
    public class ShoppingService
    {
        private readonly HttpClient _httpClient;
        private readonly DiscountGrpcService _discountService;

        public ShoppingService(HttpClient httpClient, DiscountGrpcService discountService)
        {
            _httpClient = httpClient;
            _discountService = discountService;
        }
        public async Task<object> GetShopping(string userName)
        {
            var basket = await _httpClient.GetFromJsonAsync<BasketModel>(
                $"http://basket-api:8080/api/v1/basket/{userName}");

            decimal totalPrice = 0;
            decimal totalDiscount = 0;
            if (basket != null)
            {
                foreach (var item in basket.Items)
                {
                    var coupon = await _discountService.GetDiscount(item.ProductName);

                    totalPrice += item.Price;
                    totalDiscount += coupon.Amount;
                }
            }
            return new
            {
                UserName = userName,
                Basket = basket,
                TotalPrice = totalPrice,
                TotalDiscount = totalDiscount,
                FinalPrice = totalPrice - totalDiscount
            };
        }
    }
}
