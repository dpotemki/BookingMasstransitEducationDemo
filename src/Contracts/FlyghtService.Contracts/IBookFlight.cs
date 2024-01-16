using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyghtService.Contracts
{
    public interface IBookFlight
    {
        Guid BookingId { get; }
        DateTime BookingDate { get; }

    }

    public interface FlyghtBookingConfirmed
    {
        public Guid BookingId { get; set; }
    }
    public interface FlyghtBookingRejected
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; }
    }
}
