using FinancialTracker.Application.Commands;
using FinancialTracker.Application.Interfaces.Messaging;
using FinancialTracker.Application.Interfaces.Persistence;
using FinancialTracker.Domain.Entities;
using FinancialTracker.Domain.Events;
//using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Application.Handlers
{
    public class CreateOrderHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IkafkaProducer _kafkaProducer;

        public CreateOrderHandler(IOrderRepository orderRepository, IkafkaProducer kafkaProducer)
        {
            _orderRepository = orderRepository;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<Guid> Handle(CreateOrderCommand command)
        {
            // Use the factory to create a domain entity
            var orderId = Guid.NewGuid();   // CREATED HERE
            var order = SalesOrder.CreateNew(orderId,command.Total, command.Customer);

            await _orderRepository.AddAsync(order);

            var orderCreatedEvent = new OrderCreatedEvent(order.Id, order.Total, order.Customer);
            await _kafkaProducer.PublishAsync("orders-created-topic", orderCreatedEvent);

            return order.Id;
        }

    }
}

