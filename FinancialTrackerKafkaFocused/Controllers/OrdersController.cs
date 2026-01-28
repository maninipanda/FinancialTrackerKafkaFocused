using FinancialTracker.Application.Commands;
using FinancialTracker.Application.Handlers;
using FinancialTracker.Application.Interfaces.Messaging;
using FinancialTracker.Domain.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace FinancialTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CreateOrderHandler _handler;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(CreateOrderHandler handler, ILogger<OrdersController> logger) 
            {
            _handler = handler;
            _logger = logger;
            }



        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {

            ///var json = JsonSerializer.Serialize(command, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            _logger.LogInformation( "Test-123");

            var orderId = await _handler.Handle(command);
                return Ok(new { OrderId = orderId });
           
            //var orderCreatedEvent = new OrderCreatedEvent(Guid.NewGuid(), cmd.Total);
            //await _eventBus.PublishAsync("orders-created-topic", orderCreatedEvent);
            //return Ok();
        }
    }
}
//Domain        ❌ depends on nothing
//Application   ✅ depends on Domain
//Infrastructure✅ depends on Application + Domain
//API           ✅ depends on Application + Infrastructure



//Both API and Worker share the same Kafka cluster

//Infrastructure handles Kafka, EF, and DI

//Application handles use cases and domain orchestration