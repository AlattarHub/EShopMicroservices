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
            var result = await _service.GetShopping(userName);
            return Ok(result);
        }
    }
}
