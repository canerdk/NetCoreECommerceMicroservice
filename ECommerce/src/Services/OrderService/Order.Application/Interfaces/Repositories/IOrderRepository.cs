using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Domain.AggregateModels.OrderAggregate.Order>
    {
    }
}
