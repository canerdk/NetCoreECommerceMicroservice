using AutoMapper;
using Order.Application.Features.Commands.CreateOrder;
using Order.Application.Features.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Mapping.OrderMapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Domain.AggregateModels.OrderAggregate.Order, CreateOrderCommand>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Domain.AggregateModels.OrderAggregate.Order, OrderDetailViewModel>()
                .ForMember(x => x.City, y => y.MapFrom(z => z.Address.City))
                .ForMember(x => x.Country, y => y.MapFrom(z => z.Address.Country))
                .ForMember(x => x.Street, y => y.MapFrom(z => z.Address.Street))
                .ForMember(x => x.Zipcode, y => y.MapFrom(z => z.Address.ZipCode))
                .ForMember(x => x.Date, y => y.MapFrom(z => z.OrderDate))
                .ForMember(x => x.OrderNumber, y => y.MapFrom(z => z.Id.ToString()))
                .ForMember(x => x.Status, y => y.MapFrom(z => z.OrderStatus.Name))
                .ForMember(x => x.Total, y => y.MapFrom(z => z.OrderItems.Sum(i => i.Units * i.UnitPrice)))
                .ReverseMap();


            CreateMap<OrderItem, Domain.AggregateModels.OrderAggregate.OrderItem>();




        }
    }
}
