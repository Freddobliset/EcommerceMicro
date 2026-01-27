using Order.Business.Interfaces;
using Order.Repository;
using Order.Repository.Entities;
using Order.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.ClientHttp;
using Warehouse.ClientHttp.Interfaces;
using Order.Business.Kafka;
using Warehouse.Shared.Events;

namespace Order.Business
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly IWarehouseClient _warehouseClient;
        private readonly IKafkaProducerService _kafkaProducer;

        public OrderService(OrderDbContext context, IWarehouseClient warehouseClient, IKafkaProducerService kafkaProducer)
        {
            _context = context;
            _warehouseClient = warehouseClient;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<int> CreateOrderAsync(OrderDto dto)
        {
            var productExists = await _warehouseClient.ProductExistsAsync(dto.ProductId);

            if (!productExists)
            {
                throw new Exception($"Errore: Il prodotto con ID {dto.ProductId} non è disponibile in magazzino.");
            }

            var order = new OrderEntity
            {
                CustomerEmail = dto.CustomerEmail,
                OrderDate = DateTime.UtcNow,
                Status = "Confirmed", 
                TotalAmount = dto.TotalAmount
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var kafkaMessage = new OrderCreatedEvent
            {
                ProductId = dto.ProductId,
                QuantitySold = dto.Quantity
            };

            await _kafkaProducer.SendOrderCreatedMessage(kafkaMessage);

            return order.Id;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerEmail = order.CustomerEmail,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate
            };
        }
    }
}