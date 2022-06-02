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
    internal class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentSuccessIntegrationEventHandler> _logger;

        public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
        {
            //TO-DO: Send Success Notification (Sms, Email, Push)


            _logger.LogInformation($"Order payment success with OrderId: {@event.OrderId}");

            return Task.CompletedTask;
        }
    }
}
