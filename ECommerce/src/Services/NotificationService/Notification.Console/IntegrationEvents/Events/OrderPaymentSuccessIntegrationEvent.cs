using EventBus.Base.Events;

namespace Notification.Console.IntegrationEvents.Events
{
    public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; private set; }

        public OrderPaymentSuccessIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}
