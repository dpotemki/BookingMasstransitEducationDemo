using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferService.Models
{
    internal class BookingConfirmedEvent : BookingConfirmed
    {
        public Guid BookingId { get; set; }
        public BookingItemTypeEnum ServiceType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
