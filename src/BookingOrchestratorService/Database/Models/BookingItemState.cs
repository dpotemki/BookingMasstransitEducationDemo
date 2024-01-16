using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingOrchestratorService.Database.Models
{
    public class BookingItemState
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }

        public BokkingItemType Type { get; set; }
        public BookingItemStatus State { get; set; }
        private string _refusalReason;
        public string RefusalReason
        {
            get { return _refusalReason ?? ""; }
            set { _refusalReason = value ?? ""; }
        }
        // Дополнительные данные специфичные для сервиса...

        public BookingState BookingState { get; set; }

    }

    public enum BokkingItemType
    {
        Undefined = 0,
        Transfer = 1,
        Hotel = 2,
        Flight = 3
        // Другие типы сервисов...
    }

    public enum BookingItemStatus
    {
        Pending = 0,
        Confirmed = 1,
        Rejected = 2,
        Cancelled = 3,
        Compensated = 4
    }
}
