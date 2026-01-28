using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Application.Commands
{
    //public record CreateOrderCommand(decimal Total,string customer);


    public class CreateOrderCommand
    {
      
        public decimal Total { get; set; }
        public string Customer { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
