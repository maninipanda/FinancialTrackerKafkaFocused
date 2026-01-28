using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Application.Interfaces.Messaging
{
    public interface IkafkaProducer
    {

        Task PublishAsync<T>(string topic, T message, CancellationToken ct = default);
    }

}