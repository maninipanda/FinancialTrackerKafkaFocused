using System;
using System.Collections.Generic;

namespace FinancialTracker.Infrastructure.Persistence.Data;

public partial class Order
{
    public Guid Id { get; set; }

    public string Customer { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
