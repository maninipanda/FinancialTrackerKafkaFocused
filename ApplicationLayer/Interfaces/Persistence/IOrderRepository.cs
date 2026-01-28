using FinancialTracker.Application.Interfaces.Persistence;
using FinancialTracker.Domain.Entities;

namespace FinancialTracker.Application.Interfaces.Persistence
{
    // Fix: Replace generic 'T' with the actual type you want to return.
    // Assuming 'Order' is the entity you want to return.
    public interface IOrderRepository
    {
        Task AddAsync(SalesOrder order);
        // Task<IEnumerable<OrderModel>> GetOrdersByCustomerAsync(string customerId);
        Task<SalesOrder?> GetByIdAsync(Guid id);
    }

    public interface IProcessedMessageRepository
    {
        Task<bool> TryAddAsync(ProcessedMessageM message);
        //Task AddAsync(ProcessedMessageM message);
    }
}
