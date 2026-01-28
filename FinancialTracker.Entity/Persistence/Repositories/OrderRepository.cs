using FinancialTracker.Application.Interfaces.Persistence;
using FinancialTracker.Domain.Entities;
using FinancialTracker.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace FinancialTracker.Infrastructure.Persistence.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly MyDbContext _context;
        public OrderRepository(MyDbContext context) {


            _context = context;
        
        }

        public async Task AddAsync(SalesOrder orderDetails)
        {
            // Map SalesOrder to Order entity before adding to DbContext
            // Mapping domain -> infrastructure
            var entity = new Order
            {
                // Remove Id assignment since SalesOrder.Id setter is private
             
                Id= orderDetails.Id,
                Customer = orderDetails.Customer,
                Amount = orderDetails.Amount,
                Status = orderDetails.Status,
                ErrorMessage = orderDetails.ErrorMessage,
                CreatedAt = orderDetails.CreatedAt,
                UpdatedAt = orderDetails.UpdatedAt
            };

            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();
        }

        
        public async Task<SalesOrder?> GetByIdAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return null;
            }

            // Map Order entity to SalesOrder domain entity
            // Pass all required parameters to the SalesOrder constructor
            return new SalesOrder(order.Id, order.Amount, order.Customer, order.Status);
        }



    }
}
