using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferService.Contracts
{
    public interface IBookTransfer
    {
        Guid BookingId { get; }
        DateTime BookingDate { get; }
    }

    public interface TransferBookingConfirmed
    {
        public Guid BookingId { get; set; }
    }
    public interface TransferBookingRejected
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; }
    }
}
