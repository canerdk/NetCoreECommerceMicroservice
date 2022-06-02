using EventBus.Base.Events;

namespace Notification.Console.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; private set; }
        public string ErrorMessage { get; private set; }

        public OrderPaymentFailedIntegrationEvent(int orderId, string errorMessage)
        {
            OrderId = orderId;
            ErrorMessage = errorMessage;
        }
    }
}
