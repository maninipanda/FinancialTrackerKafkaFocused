using FinancialTracker.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Domain.Entities
{
    public class SalesOrder
    {


        // Domain safety: Making it private ensures no one can accidentally create an empty order in your application code, bypassing business rules.DDD principle
        public Guid Id { get; private set; }
        public decimal Total { get; private set; }
        public string Customer { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } 

        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // EF Core requires a private parameterless constructor
        private SalesOrder() { }

        public SalesOrder(Guid id, decimal total, string customer, string status)
        {
            if (total <= 0) throw new ArgumentException("Total must be positive", nameof(total));

            Id = id;
            Total = total;
            Customer = customer;
            Status = status;
            CreatedAt = DateTime.UtcNow; // if desired
            Amount = total; // if Amount should mirror Total
        }
        // Factory method for creating a new order
        public static SalesOrder CreateNew(Guid Id, decimal total, string customer)
        {
            return new SalesOrder(Id, total, customer, "Pending");
        }


    }


    public class ProcessedMessageM
    {
        public string MessageKey { get; set; } 

        public long Offset { get; set; }

        public string Topic { get; set; }
        public DateTime ProcessedAt { get; set; }

    }
}