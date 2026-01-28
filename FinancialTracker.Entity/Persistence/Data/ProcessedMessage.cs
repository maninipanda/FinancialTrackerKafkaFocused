using System;
using System.Collections.Generic;


namespace FinancialTracker.Infrastructure.Persistence.Data;

public partial class ProcessedMessage
{
    public int Id { get; set; }

    public string MessageKey { get; set; } = null!;

    public long Offset { get; set; }

    public string Topic { get; set; } = null!;

    public DateTime ProcessedAt { get; set; }
}
