using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notification.Console.IntegrationEvents.EventHandlers;
using Notification.Console.IntegrationEvents.Events;

ServiceCollection services = new ServiceCollection();
ConfigureServices(services);

var sp = services.BuildServiceProvider();
IEventBus eventBus = sp.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();
eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();

Console.WriteLine("Application is running..");

Console.ReadLine();


void ConfigureServices(ServiceCollection services)
{
    services.AddLogging(cfg =>
    {
        cfg.AddConsole();
    });

    services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
    services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

    services.AddSingleton<IEventBus>(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "PaymentService",
            EventBusType = EventBusType.RabbitMQ
        };

        return EventBusFactory.Create(config, sp);
    });
}