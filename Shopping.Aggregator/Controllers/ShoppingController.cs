using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Services;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly ShoppingService _service;

        public ShoppingController(ShoppingService service)
        {
            _service = service;
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetShopping(string userName)
        {
            var basket = await _service.GetBasket(userName);
            var catalog = await _service.GetCatalog();

            var response = new
            {
                UserName = userName,
                Basket = basket,
                Products = catalog
            };

            return Ok(response);
        }
    }
}
