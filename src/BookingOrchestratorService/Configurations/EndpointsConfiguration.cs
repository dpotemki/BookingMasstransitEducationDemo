using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingOrchestratorService.Configurations
{
    public class EndpointsConfiguration
    {

        public string BookingStateMachineQueueName { get; set; }
        public string HotelServiceQueueName { get; set; }
        public string FlightServiceQueueName { get; set; }
        public string TransferServiceQueueName { get; set; }
    }
}
