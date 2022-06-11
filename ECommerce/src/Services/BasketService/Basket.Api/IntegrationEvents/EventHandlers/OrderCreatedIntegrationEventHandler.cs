using Basket.Api.Core.Application.Repository;
using Basket.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace Basket.Api.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(IBasketRepository basketRepository, ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            _basketRepository = basketRepository;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            _logger.LogInformation("--- Handling integration event:", @event.Id);

            await _basketRepository.DeleteBasketAsync(@event.UserId.ToString());
        }
    }
}
