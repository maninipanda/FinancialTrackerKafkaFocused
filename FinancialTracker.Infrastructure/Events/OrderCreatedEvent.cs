using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Domain.Events
{

    public record OrderCreatedEvent( Guid OrderId, decimal Total,string customer);

}
