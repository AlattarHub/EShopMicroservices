using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using EventBus.Messages.Events;
using MassTransit;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;        
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService,IPublishEndpoint publishEndpoint)
        {
            _repository = repository;            
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var basket = await _repository.GetBasket(username);

            return Ok(basket ?? new ShoppingCart(username));
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteBasket(string username)
        {
            await _repository.DeleteBasket(username);
            return Ok();
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckoutEvent basketCheckout)
        {
            var basket = await _repository.GetBasket(basketCheckout.UserName);

            if (basket == null)
                return BadRequest();

            var eventMessage = new BasketCheckoutEvent
            {
                UserName = basketCheckout.UserName,
                TotalPrice = basket.TotalPrice,

                FirstName = basketCheckout.FirstName,
                LastName = basketCheckout.LastName,
                EmailAddress = basketCheckout.EmailAddress,

                AddressLine = basketCheckout.AddressLine,
                Country = basketCheckout.Country,
                State = basketCheckout.State,
                ZipCode = basketCheckout.ZipCode,

                CardName = basketCheckout.CardName,
                CardNumber = basketCheckout.CardNumber,
                Expiration = basketCheckout.Expiration,
                CVV = basketCheckout.CVV,
                PaymentMethod = basketCheckout.PaymentMethod
            };

            await _publishEndpoint.Publish(eventMessage);

            await _repository.DeleteBasket(basketCheckout.UserName);

            return Accepted();

        }
    }
}
