using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using Notification.Console.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Console.IntegrationEvents.EventHandlers
{
    internal class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

        public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            //TO-DO: Send Fail Notification (Sms, Email, Push)


            _logger.LogInformation($"Order payment failed with OrderId: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");

            return Task.CompletedTask;
        }
    }
}
