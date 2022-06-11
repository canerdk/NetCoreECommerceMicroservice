using Basket.Api.Core.Application.Repository;
using Basket.Api.Core.Application.Services;
using Basket.Api.Extensions;
using Basket.Api.Infrastructure.Repository;
using Basket.Api.IntegrationEvents.EventHandlers;
using Basket.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();

builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));
builder.Services.ConfigureConsul(builder.Configuration);

builder.Services.AddSingleton<IEventBus>(sp => {
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService",
        EventBusType = EventBusType.RabbitMQ
    };

    return EventBusFactory.Create(config, sp);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.RegisterWithConsul(app.Configuration);

IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();
