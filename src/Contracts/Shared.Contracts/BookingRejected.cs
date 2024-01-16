using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    public interface BookingRejected
    {
        public Guid BookingId { get; set; }
        public BookingItemTypeEnum ServiceType { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
