using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order.Shared;
namespace Order.Business.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(OrderDto orderDto);
        Task<OrderDto?> GetOrderByIdAsync(int id);
    }
}
