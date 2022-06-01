using EventBus.Base.Abstraction;
using EventBus.UnitTest.Events.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.UnitTest.Events.EventHandler
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            Console.WriteLine("Handler method worked with id:" + @event.Id);
            var here = "Handler method worked with id:" + @event.Id;
            return Task.CompletedTask;
        }
    }
}
