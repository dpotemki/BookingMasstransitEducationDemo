using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelService.Contracts
{
    public interface IBookHotel
    {
        Guid BookingId { get; }
        DateTime BookingDate { get; }
    }
    public interface HotelBookingConfirmed
    {
        public Guid BookingId { get; set; }
    }
    public interface HotelBookingRejected
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; }
    }
}
