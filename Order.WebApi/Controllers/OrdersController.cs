using Microsoft.AspNetCore.Mvc;
using Order.Business.Interfaces;
using Order.Shared;

namespace Order.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto dto)
        {
            var id = await _orderService.CreateOrderAsync(dto);
            return Ok(new { Message = "Ordine creato!", OrderId = id });
        }
    }
}