using Basket.Api.Core.Application.Repository;
using Basket.Api.Core.Application.Services;
using Basket.Api.Core.Domain.Models;
using Basket.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketRepository basketRepository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
        {
            _basketRepository = basketRepository;
            _identityService = identityService;
            _eventBus = eventBus;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            return Ok(basket);
        }

        [HttpPost]
        [Route("update")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync([FromBody]CustomerBasket basket)
        {
            return Ok(await _basketRepository.UpdateBasketAsync(basket));
        }

        [HttpPost]
        [Route("additem")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem basketItem)
        {
            var userId = _identityService.GetUserName().ToString();

            var basket = await _basketRepository.GetBasketAsync(userId);

            if (basket == null)
                basket = new CustomerBasket(userId);

            basket.Items.Add(basketItem);
            await _basketRepository.UpdateBasketAsync(basket);

            return Ok();
        }

        [HttpPost]
        [Route("checkout")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout, [FromHeader(Name = "x-request-id")] string requestId)
        {
            var userId = basketCheckout.Buyer;

            var basket = await _basketRepository.GetBasketAsync(userId);

            if (basket == null)
                return BadRequest();

            var userName = _identityService.GetUserName();

            var eventMessage = new OrderCreatedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street, basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName, basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basket);
            try
            {
                _eventBus.Publish(eventMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration event");
                throw;
            }

            return Accepted();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }
    }
}
