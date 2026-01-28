using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace FinancialTrackerKafkaFocused.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
    }
}
// financialtrackerkafkafocused-kafka-1
// docker exec -it financialtrackerkafkafocused-kafka-1 bash
// docker logs -f financialtrackerkafkafocused-kafka-1

//docker compose down      # stop old containers
//docker rm kafka zookeeper # remove old containers
//docker compose up -d     # start fresh
//docker ps                # verify both containers are running
//docker logs kafka         # check Kafka logs