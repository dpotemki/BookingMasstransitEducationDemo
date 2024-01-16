using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    public interface BookingConfirmed
    {
        public Guid BookingId { get; set; }
        public BookingItemTypeEnum ServiceType { get; set; }
        public DateTime Timestamp { get; set; }
    }
    

    
}
