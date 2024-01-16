using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    public interface CompensateBooking
    {
        public Guid BookingId { get; set; }
        public BookingItemTypeEnum ServiceType { get; set; }
        // Может содержать дополнительные данные для компенсационной транзакции
    }
}
