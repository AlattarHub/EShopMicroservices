using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContextSeed
    {
        public static async Task SeedDataAsync(IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();

            if (!existProduct)
            {
                await productCollection.InsertManyAsync(GetPreconfiguredProducts());
            }
        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            return new List<Product>()
            {
                new Product
                {
                    Name = "iPhone 14",
                    Category = "Smart Phone",
                    Summary = "Apple smartphone",
                    Description = "Latest Apple iPhone",
                    ImageFile = "iphone.png",
                    Price = 1200
                },
                new Product
                {
                    Name = "Samsung Galaxy",
                    Category = "Smart Phone",
                    Summary = "Samsung smartphone",
                    Description = "Latest Samsung phone",
                    ImageFile = "samsung.png",
                    Price = 1000
                }
            };
        }
    }
}