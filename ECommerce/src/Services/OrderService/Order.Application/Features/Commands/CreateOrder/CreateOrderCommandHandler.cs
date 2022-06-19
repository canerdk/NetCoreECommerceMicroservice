using EventBus.Base.Abstraction;
using MediatR;
using Order.Application.IntegrationEvents;
using Order.Application.Interfaces.Repositories;
using Order.Domain.AggregateModels.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventBus _eventBus;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var address = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode);
            Order.Domain.AggregateModels.OrderAggregate.Order dbOrder = new(request.UserName, address,
                request.CardTypeId, request.CardNumber, request.CardSecurityNumber, request.CardHolderName,
                request.CardExpiration, null);

            request.OrderItems.ToList().ForEach(i => dbOrder.AddOrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.PictureUrl, i.Units));

            await _orderRepository.AddAsync(dbOrder);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(request.UserName);

            _eventBus.Publish(orderStartedIntegrationEvent);

            return true;
        }
    }
}
