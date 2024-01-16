using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferService.Configuration
{
    public class RabbitMqConfiguration
    {
        public string? Hostname { get; set; }
        public string? VirtualHost { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
