using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using Payment.Api.IntegrationEvents.Events;

namespace Payment.Api.IntegrationEvents.EventHandlers
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

        public OrderStartedIntegrationEventHandler(IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            bool paymentFlag = true;

            IntegrationEvent paymentEvent = paymentFlag ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId) : new OrderPaymentFailedIntegrationEvent(@event.OrderId, "Fake error");

            _logger.LogInformation($"OrderStartedIntegrationEventHandler in PaymentService if fired with PaymentSuccess: {paymentFlag}, orderId: {@event.OrderId}");

            _eventBus.Publish(paymentEvent);

            return Task.CompletedTask;
        }
    }
}
