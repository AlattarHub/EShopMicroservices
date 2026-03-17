using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services
{
    public class ShoppingService
    {
        private readonly HttpClient _httpClient;

        public ShoppingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<BasketModel?> GetBasket(string userName)
        {
            return await _httpClient.GetFromJsonAsync<BasketModel>(
                $"http://basket-api:8080/api/v1/basket/{userName}");
        }

        public async Task<List<CatalogModel>?> GetCatalog()
        {
            return await _httpClient.GetFromJsonAsync<List<CatalogModel>>(
                "http://catalog-api:8080/api/v1/catalog");
        }
    }
}
